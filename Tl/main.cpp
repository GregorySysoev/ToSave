#include <iostream>
#include "word.h"

void findNReplace(word& s1, word& s2, word& s3) {
    int pos = s1.findWord(s2);
    while (-1 != pos) {
        s1 = s1.substr(0, pos) + s3 + s1.substr(pos + s2.getLen(), s1.getLen());
        pos = s1.findWord(s2);
    }
}
int main() {
    string s1 = "kot pes papug";
    string s2 = "pes";
    string s3 = "papug";
    word a1(4, s1);
    word a2(4, s2);
    word a3(4, s3);
    int pos = a1.findWord(a2);
    //a1 = a1.substr(0, pos) + a3;
    findNReplace(a1,a2,a3);
    cout << a1 << endl;
    system("pause");
    return 0;
}