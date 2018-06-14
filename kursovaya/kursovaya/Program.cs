using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace kursovaya
{
    public class TimSort<T> 
    {
        
        private const int MIN_MERGE = 32;
        private T[] a;
        private IComparer<T> c;
        private static int MIN_GALLOP = 7;
        private int minGallop = MIN_GALLOP;
        private static int INITIAL_TMP_STORAGE_LENGTH = 256;
        private T[] tmp; 
        private int stackSize = 0;
        private int[] runBase;
        private int[] runLen;

        private TimSort(T[] a, IComparer<T> c)
        {
            this.a = a;
            this.c = c;
            var len = a.Length;
            var newArray = (T[])new T[len < 2 * INITIAL_TMP_STORAGE_LENGTH ?
                                            len >> 1 : INITIAL_TMP_STORAGE_LENGTH];
            tmp = newArray;
            int stackLen = (len < 120 ? 5 :
                            len < 1542 ? 10 :
                            len < 119151 ? 19 : 40);
            runBase = new int[stackLen];
            runLen = new int[stackLen];

        }
        static void sort(T[] a, IComparer<T> c)
        {
            sort(a, 0, a.Length, c);
        }
        static void sort(T[] a, int lo, int hi, IComparer<T> c)
        {
            if (c == null)
            {
                var work = a.ToList<T>();
                work.Sort();
                a = work.ToArray<T>();
                return;
            }

            rangeCheck(a.Length, lo, hi);
            int nRemaining = hi - lo;
            if (nRemaining < 2)
                return; 
            if (nRemaining < MIN_MERGE)
            {
                int initRunLen = countRunAndMakeAscending(a, lo, hi, c);
                binarySort(a, lo, hi, lo + initRunLen, c);
                return;
            }
            var ts = new TimSort<T>(a, c);
            int minRun = minRunLength(nRemaining);
            do
            {
                int runLen = countRunAndMakeAscending(a, lo, hi, c);

                if (runLen < minRun)
                {
                    int force = nRemaining <= minRun ? nRemaining : minRun;
                    binarySort(a, lo, lo + force, lo + runLen, c);
                    runLen = force;
                }

                ts.pushRun(lo, runLen);
                ts.mergeCollapse();

                lo += runLen;
                nRemaining -= runLen;
            } while (nRemaining != 0);

            ts.mergeForceCollapse();
        }
        
        private static void binarySort(T[] a, int lo, int hi, int start,
                                           IComparer<T> c)
        {
            if (start == lo)
                start++;
            for (; start < hi; start++)
            {
                var pivot = a[start];

                int left = lo;
                int right = start;
                while (left < right)
                {
                    int mid = (left + right) >> 1;
                    if (c.Compare(pivot, a[mid]) < 0)
                        right = mid;
                    else
                        left = mid + 1;
                }
                int n = start - left;  
                switch (n)
                {
                    case 2:
                        a[left + 2] = a[left + 1];
                        goto case 1;
                    case 1:
                        a[left + 1] = a[left];
                        break;
                    default:
                        Array.Copy(a, left, a, left + 1, n);
                        break;
                }
                a[left] = pivot;
            }
        }
        private static int countRunAndMakeAscending(T[] a, int lo, int hi,
                                                        IComparer<T> c)
        {
            //assert lo < hi;
            int runHi = lo + 1;
            if (runHi == hi)
                return 1;

            if (c.Compare(a[runHi++], a[lo]) < 0)
            { 
                while (runHi < hi && c.Compare(a[runHi], a[runHi - 1]) < 0)
                    runHi++;
                reverseRange(a, lo, runHi);
            }
            else
            {                              // Ascending
                while (runHi < hi && c.Compare(a[runHi], a[runHi - 1]) >= 0)
                    runHi++;
            }

            return runHi - lo;
        }
        private static void reverseRange(T[] a, int lo, int hi)
        {
            hi--;
            while (lo < hi)
            {
                var t = a[lo];
                a[lo++] = a[hi];
                a[hi--] = t;
            }
        }
       
        private static int minRunLength(int n)
        {
            int r = 0;     
            while (n >= MIN_MERGE)
            {
                r |= (n & 1);
                n >>= 1;
            }
            return n + r;
        }
        
        private void pushRun(int runBase, int runLen)
        {
            this.runBase[stackSize] = runBase;
            this.runLen[stackSize] = runLen;
            stackSize++;
        }

        private void mergeCollapse()
        {
            while (stackSize > 1)
            {
                int n = stackSize - 2;
                if (n > 0 && runLen[n - 1] <= runLen[n] + runLen[n + 1])
                {
                    if (runLen[n - 1] < runLen[n + 1])
                        n--;
                    mergeAt(n);
                }
                else if (runLen[n] <= runLen[n + 1])
                {
                    mergeAt(n);
                }
                else
                {
                    break; 
                }
            }
        }
        private void mergeForceCollapse()
        {
            while (stackSize > 1)
            {
                int n = stackSize - 2;
                if (n > 0 && runLen[n - 1] < runLen[n + 1])
                    n--;
                mergeAt(n);
            }
        }

        private void mergeAt(int i)
        {
            
            int base1 = runBase[i];
            int len1 = runLen[i];
            int base2 = runBase[i + 1];
            int len2 = runLen[i + 1];
            runLen[i] = len1 + len2;
            if (i == stackSize - 3)
            {
                runBase[i + 1] = runBase[i + 2];
                runLen[i + 1] = runLen[i + 2];
            }
            stackSize--;


            int k = gallopRight(a[base2], a, base1, len1, 0, c);
            base1 += k;
            len1 -= k;
            if (len1 == 0)
                return;

            
            len2 = gallopLeft(a[base1 + len1 - 1], a, base2, len2, len2 - 1, c);
            if (len2 == 0)
                return;

            if (len1 <= len2)
                mergeLo(base1, len1, base2, len2);
            else
                mergeHi(base1, len1, base2, len2);
        }

        
        private static int gallopLeft(T key, T[] a, int basei, int len, int hint,
                                          IComparer<T> c)
        {
            int lastOfs = 0;
            int ofs = 1;
            if (c.Compare(key, a[basei + hint]) > 0)
            {
                int maxOfs = len - hint;
                while (ofs < maxOfs && c.Compare(key, a[basei + hint + ofs]) > 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)   
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                lastOfs += hint;
                ofs += hint;
            }
            else
            { 
                int maxOfs = hint + 1;
                while (ofs < maxOfs && c.Compare(key, a[basei + hint - ofs]) <= 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)   
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                int tmp = lastOfs;
                lastOfs = hint - ofs;
                ofs = hint - tmp;
            }
            lastOfs++;
            while (lastOfs < ofs)
            {
                int m = lastOfs + ((ofs - lastOfs) >> 1);

                if (c.Compare(key, a[basei + m]) > 0)
                    lastOfs = m + 1;  
                else
                    ofs = m;          
            }
            return ofs;
        }
        
        private static int gallopRight(T key, T[] a, int basei, int len,
                                           int hint, IComparer<T> c)
        {

            int ofs = 1;
            int lastOfs = 0;
            if (c.Compare(key, a[basei + hint]) < 0)
            {
                int maxOfs = hint + 1;
                while (ofs < maxOfs && c.Compare(key, a[basei + hint - ofs]) < 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)   
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                int tmp = lastOfs;
                lastOfs = hint - ofs;
                ofs = hint - tmp;
            }
            else
            { 
                int maxOfs = len - hint;
                while (ofs < maxOfs && c.Compare(key, a[basei + hint + ofs]) >= 0)
                {
                    lastOfs = ofs;
                    ofs = (ofs << 1) + 1;
                    if (ofs <= 0)   // int overflow
                        ofs = maxOfs;
                }
                if (ofs > maxOfs)
                    ofs = maxOfs;

                lastOfs += hint;
                ofs += hint;
            }
            lastOfs++;
            while (lastOfs < ofs)
            {
                int m = lastOfs + ((ofs - lastOfs) >> 1);

                if (c.Compare(key, a[basei + m]) < 0)
                    ofs = m;          // key < a[b + m]
                else
                    lastOfs = m + 1;  // a[b + m] <= key
            }
            return ofs;
        }
        
        private void mergeLo(int base1, int len1, int base2, int len2)
        {
            var a = this.a; // For performance
            var tmp = ensureCapacity(len1);
            Array.Copy(a, base1, tmp, 0, len1);

            int cursor1 = 0;       // Indexes into tmp array
            int cursor2 = base2;   // Indexes int a
            int dest = base1;      // Indexes int a

            a[dest++] = a[cursor2++];
            if (--len2 == 0)
            {
                Array.Copy(tmp, cursor1, a, dest, len1);
                return;
            }
            if (len1 == 1)
            {
                Array.Copy(a, cursor2, a, dest, len2);
                a[dest + len2] = tmp[cursor1]; // Last elt of run 1 to end of merge
                return;
            }

            var c = this.c;  // Use local variable for performance
            int minGallop = this.minGallop;    //  "    "       "     "      "
        outer:
            while (true)
            {
                int count1 = 0; // Number of times in a row that first run won
                int count2 = 0; // Number of times in a row that second run won

                do
                {
                    if (c.Compare(a[cursor2], tmp[cursor1]) < 0)
                    {
                        a[dest++] = a[cursor2++];
                        count2++;
                        count1 = 0;
                        if (--len2 == 0)
                            goto outer;
                    }
                    else
                    {
                        a[dest++] = tmp[cursor1++];
                        count1++;
                        count2 = 0;
                        if (--len1 == 1)
                            goto outer;
                    }
                } while ((count1 | count2) < minGallop);

                
                do
                {
                    count1 = gallopRight(a[cursor2], tmp, cursor1, len1, 0, c);
                    if (count1 != 0)
                    {
                        Array.Copy(tmp, cursor1, a, dest, count1);
                        dest += count1;
                        cursor1 += count1;
                        len1 -= count1;
                        if (len1 <= 1) // len1 == 1 || len1 == 0
                            goto outer;
                    }
                    a[dest++] = a[cursor2++];
                    if (--len2 == 0)
                        goto outer;

                    count2 = gallopLeft(tmp[cursor1], a, cursor2, len2, 0, c);
                    if (count2 != 0)
                    {
                        Array.Copy(a, cursor2, a, dest, count2);
                        dest += count2;
                        cursor2 += count2;
                        len2 -= count2;
                        if (len2 == 0)
                            goto outer;
                    }
                    a[dest++] = tmp[cursor1++];
                    if (--len1 == 1)
                        goto outer;
                    minGallop--;
                } while (count1 >= MIN_GALLOP | count2 >= MIN_GALLOP);
                if (minGallop < 0)
                    minGallop = 0;
                minGallop += 2;  
            }  
            this.minGallop = minGallop < 1 ? 1 : minGallop;  

            if (len1 == 1)
            {
                Array.Copy(a, cursor2, a, dest, len2);
                a[dest + len2] = tmp[cursor1]; //  Last elt of run 1 to end of merge
            }
            else if (len1 == 0)
            {
                throw new ArgumentException(
                    "Comparison method violates its general contract!");
            }
            else
            {
                //assert len2 == 0;
                //assert len1 > 1;
                Array.Copy(tmp, cursor1, a, dest, len1);
            }
        }
        
        private void mergeHi(int base1, int len1, int base2, int len2)
        {
            
            T[] a = this.a; // For performance
            T[] tmp = ensureCapacity(len2);
            Array.Copy(a, base2, tmp, 0, len2);

            int cursor1 = base1 + len1 - 1;  // Indexes into a
            int cursor2 = len2 - 1;          // Indexes into tmp array
            int dest = base2 + len2 - 1;     // Indexes into a

            a[dest--] = a[cursor1--];
            if (--len1 == 0)
            {
                Array.Copy(tmp, 0, a, dest - (len2 - 1), len2);
                return;
            }
            if (len2 == 1)
            {
                dest -= len1;
                cursor1 -= len1;
                Array.Copy(a, cursor1 + 1, a, dest + 1, len1);
                a[dest] = tmp[cursor2];
                return;
            }

            var c = this.c;  
            int minGallop = this.minGallop;    
        outer:
            while (true)
            {
                int count1 = 0; // Number of times in a row that first run won
                int count2 = 0; // Number of times in a row that second run won

               
                do
                {
                    if (c.Compare(tmp[cursor2], a[cursor1]) < 0)
                    {
                        a[dest--] = a[cursor1--];
                        count1++;
                        count2 = 0;
                        if (--len1 == 0)
                            goto outer;
                    }
                    else
                    {
                        a[dest--] = tmp[cursor2--];
                        count2++;
                        count1 = 0;
                        if (--len2 == 1)
                            goto outer;
                    }
                } while ((count1 | count2) < minGallop);

                
                do
                {
                    //assert len1 > 0 && len2 > 1;
                    count1 = len1 - gallopRight(tmp[cursor2], a, base1, len1, len1 - 1, c);
                    if (count1 != 0)
                    {
                        dest -= count1;
                        cursor1 -= count1;
                        len1 -= count1;
                        Array.Copy(a, cursor1 + 1, a, dest + 1, count1);
                        if (len1 == 0)
                            goto outer;
                    }
                    a[dest--] = tmp[cursor2--];
                    if (--len2 == 1)
                        goto outer;

                    count2 = len2 - gallopLeft(a[cursor1], tmp, 0, len2, len2 - 1, c);
                    if (count2 != 0)
                    {
                        dest -= count2;
                        cursor2 -= count2;
                        len2 -= count2;
                        Array.Copy(tmp, cursor2 + 1, a, dest + 1, count2);
                        if (len2 <= 1)  // len2 == 1 || len2 == 0
                            goto outer;
                    }
                    a[dest--] = a[cursor1--];
                    if (--len1 == 0)
                        goto outer;
                    minGallop--;
                } while (count1 >= MIN_GALLOP | count2 >= MIN_GALLOP);
                if (minGallop < 0)
                    minGallop = 0;
                minGallop += 2;  // Penalize for leaving gallop mode
            }  // End of "outer" loop
            this.minGallop = minGallop < 1 ? 1 : minGallop;  // Write back to field

            if (len2 == 1)
            {
                //assert len1 > 0;
                dest -= len1;
                cursor1 -= len1;
                Array.Copy(a, cursor1 + 1, a, dest + 1, len1);
                a[dest] = tmp[cursor2];  // Move first elt of run2 to front of merge
            }
            else if (len2 == 0)
            {
                throw new ArgumentException(
                    "Comparison method violates its general contract!");
            }
            else
            {
                //assert len1 == 0;
                //assert len2 > 0;
                Array.Copy(tmp, 0, a, dest - (len2 - 1), len2);
            }
        }

        
        private T[] ensureCapacity(int minCapacity)
        {
            if (tmp.Length < minCapacity)
            {
                // Compute smallest power of 2 > minCapacity
                int newSize = minCapacity;
                newSize |= newSize >> 1;
                newSize |= newSize >> 2;
                newSize |= newSize >> 4;
                newSize |= newSize >> 8;
                newSize |= newSize >> 16;
                newSize++;

                if (newSize < 0) // Not bloody likely!
                    newSize = minCapacity;
                else
                    //newSize = Math.Min(newSize, a.Length >>> 1);
                    newSize = Math.Min(newSize, a.Length >> 1);

                //@SuppressWarnings({"unchecked", "UnnecessaryLocalVariable"})
                var newArray = (T[])new T[newSize];
                tmp = newArray;
            }
            return tmp;
        }

        private static void rangeCheck(int arrayLen, int fromIndex, int toIndex)
        {
            if (fromIndex > toIndex)
                throw new ArgumentException("fromIndex(" + fromIndex +
                           ") > toIndex(" + toIndex + ")");
            if (fromIndex < 0)
                throw new ArgumentOutOfRangeException("fromIndex", fromIndex.ToString());
            if (toIndex > arrayLen)
                throw new ArgumentOutOfRangeException("toIndex", toIndex.ToString());
        }
    }

    public class GenerateFile
    {
        public void CreateFile(byte status, int count)
        {
            switch (status)
            {
                case 1:
                    string path = @"C:\Users\User\Documents\CursWork\Best";
                    string p = path;
                    int copy = 1;
                    while (File.Exists(path))
                    {
                        p = path + copy.ToString();
                        copy++;
                    }

                    Random rand = new Random();
                    int temp = rand.Next(100);
                    int prev = 0;

                    int GetMinrun(int n)
                    {
                        int r = 0;           /* станет 1 если среди сдвинутых битов будет хотя бы 1 ненулевой */
                        while (n >= 64)
                        {
                            r |= n & 1;
                            n >>= 1;
                        }
                        return n + r;
                    }
                    int mran = GetMinrun(count);

                    using (StreamWriter sw = new StreamWriter(p))
                    {
                        for (int i = 0, j = 0; i < count; i++, j++)
                        {
                            if (j != mran)
                            {
                                temp = rand.Next(2 * i);
                                if (prev > temp) temp = prev + 1;
                                sw.WriteLine(temp);
                                prev = temp;
                            }
                            else
                            {
                                j = 0;
                                temp = rand.Next(10 * i);
                                sw.WriteLine(temp);
                            }
                        }
                        sw.Close();
                    }
                    break;

                case 2:
                    rand = new Random();
                    string path1 = @"C:\Users\User\Documents\CursWork\Average";
                    string p1 = path1;
                    int copy1 = 1;
                    while (File.Exists(path1))
                    {
                        p1 = path1 + copy1.ToString();
                        copy1++;
                    }
                    using (StreamWriter sw = new StreamWriter(p1))
                    {
                        for (int i = 0; i < count; i++)
                        {
                            temp = rand.Next(count);
                            sw.WriteLine(temp);
                        }
                        sw.Close();
                    }
                    break;

                case 3:
                    string path2 = @"C:\Users\User\Documents\CursWork\Worst";
                    string p2 = path2;
                    int copy2 = 1;
                    while (File.Exists(path2))
                    {
                        p2 = path2 + copy2.ToString();
                        copy2++;
                    }
                    using (StreamWriter sw = new StreamWriter(p2))
                    {
                        sw.Close();
                    }
                    break;
            }
        }
    }

    class Program
    {

        static void Main(string[] args)
        {
            Application.Run(new SortForm());  
        }
    }
}
