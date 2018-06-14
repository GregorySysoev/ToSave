//
// Created by User on 29.05.2018.
//

#include "word.h"

cell::cell(int len) {
    arr = new char[len];
    for (int i = 0; i < len; i++) {
        arr[i] = '^';
    }
}

void cell::clearCell() {
    if (this->left) this->left->right = nullptr;
    if (this->right) this->right->left = nullptr;
    delete[] arr;
    arr = nullptr;
}

word::word(int l, string &str) {

    if (l > 1) len = l;
    if (0 == str.length()) {
        head = nullptr;
        tail = nullptr;
    }
    else {
        int i = 0;
        int j = 0;

        cell *prev = nullptr;
        cell *cur = new cell(len);

        head = cur;

        while (i < str.length()) {
            if ((i % len == 0) && (i != 0)) {
                if (prev != nullptr) cur->left = prev;
                cur->right = new cell(len);
                prev = cur;
                cur = cur->right;
                j = 0;
            }
            cur->arr[j] = str[i];
            i++;
            j++;
        }

        cur->left = prev;
        tail = cur;
    }
}

int word::getLen() {
    int count = 0;
    cell* cur = head;
    while (cur != nullptr) {
        for (int i = 0; i < len; i++) {
            if (cur->arr[i] == '^') return count;
            count++;
        }
        cur = cur->right;
    }
    return count;
}

ostream& operator << (ostream& os, word& str) {
    cell* pointerToHead = str.head;
    while (pointerToHead != nullptr) {
        for (int i = 0; i < str.len; i++) {
            if (pointerToHead->arr[i] == '^') return os;
            os << pointerToHead->arr[i];
        }
        pointerToHead = pointerToHead->right;
    }
    return os;
}

word::word(const word &a) {

    len = a.len;
    if (nullptr == a.head)
    {
        head = nullptr;
        tail = nullptr;
    }
    else {

        cell* pointerToHead = a.head;
        cell* prev = nullptr;
        cell* cur = new cell(len);

        for (int i = 0; i < len; i++) cur->arr[i] = pointerToHead->arr[i];
        pointerToHead = pointerToHead->right;
        head = cur;

        while (pointerToHead != nullptr) {
            if (prev != nullptr) cur->left = prev;
            cur->right = new cell(len);
            cur = cur->right;
            for (int i = 0; i < len; i++) cur->arr[i] = pointerToHead->arr[i];
            prev = cur;
            pointerToHead = pointerToHead->right;
        }
        tail = cur;
    }
}

int word::findWord(word& str) {
    if ((0 != str.getLen()) && (0 != getLen())) {
        int index = 0;
        int coincedent = 0;
        cell* cur = this->head;
        cell* curStr = str.head;

        while (index <= getLen() - str.getLen() + 1) {
            int coinceded = index;
            cell* coincededCur = cur;
            cell* coincededCurStr = curStr;
            while ((coincededCur != nullptr) && (coincededCurStr != nullptr) &&
                    (coincededCur->arr[coinceded % len] == coincededCurStr->arr[coincedent % str.len])) {
                coinceded++;
                coincedent++;
                if (0 == coinceded % len) coincededCur = coincededCur->right;
                if (0 == coincedent % str.len) coincededCurStr = coincededCurStr->right;
            }
            if (coincedent >= str.getLen()) return index;
            else coincedent = 0;

            index++;
            if ((index % len == 0)) {
                cur = cur->right;
            }
        }
    }
    return -1;
}

word::~word() {
    cell* cur = head;
    cell* prev = cur;
    while (cur != nullptr) {
        cur = cur->right;
        prev->clearCell();
        prev = cur;
    }
    head = nullptr;
    tail = nullptr;
}

word &word::substr(int num, int count) {
    if ((num > this->getLen()) || (count <= 0) || (num < 0) || (0 == this->getLen())) {
        string s = "";
        word * res = new word(len, s);
        return * res;
    }
    else {
        int skipCell = num / len;
        string str = "";
        cell* cur = head;
        cell* prev = cur;

        for (skipCell; skipCell != 0; skipCell--) {
            cur = cur->right;
        }
        while ((count != 0) && (cur != nullptr)) {
            str += cur->arr[num % len];
            num++;
            count--;
            if (0 == num % len) {
                prev = cur;
                cur = cur->right;
            }
        }
        word* res = new word(len, str);
        return* res;
    }
}

word &word::operator=(word &a) {
    this->~word();
    word* res = new word(a);
    head = res->head;
    tail = res->tail;
    len = res->len;
    return* this;
}

word &word::operator=(string s) {
    word* res = new word(len, s);
    this->~word();
    head = res->head;
    tail = res->tail;
    return* this;
}

word &word::operator+(word &a) {

    if (0 == a.getLen()) return* this;
    else if (nullptr == head) {
        this->~word();
        return a;
    }
    else {
        word * result = new word(*this);
        cell* curThis = result->tail;
        cell* curA = a.head;

        if (nullptr == curA) return *result;

        cell* prev = curThis;
        int indexInThis = getLen();
        int indexInA = 0;

        while (indexInA <= a.getLen()) {
            if (0 == indexInThis % len) {
                curThis->right = new cell(len);
                curThis = curThis->right;
                curThis->left = prev;
                prev = curThis;
            }
            curThis->arr[indexInThis % len] = curA->arr[indexInA % a.len];
            indexInA++;
            indexInThis++;
            if (0 == indexInA % a.len) {
                curA = curA->right;
            }
        }
        result->tail = curThis;
        return * result;
    }
}

word &word::operator+(string a) {
    word res(1, a);
    *this + res;
    return *this;
    if (nullptr == head) return res;
    cell* curThis = tail;
    cell* prev = curThis;
    int indexInThis = getLen() % len;
    int indexInA = 0;
    while (indexInA <= a.length()) {
        if ((0 == indexInThis % len) && (0 != indexInThis)) {
            curThis->right = new cell(len);
            curThis = curThis->right;
            curThis->left = prev;
            prev = curThis;
        }
        curThis->arr[indexInThis % len] = a[indexInA];
        indexInA++;
        indexInThis++;
    }
}


