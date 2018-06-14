//
// Created by User on 29.05.2018.
//

#ifndef TL_WORD_H
#define TL_WORD_H


#include <iostream>
#include <ostream>
#include <cstring>
#include <string>
#include <cstdlib>

using std::ostream;
using std::string;
using std::cout;
using std::endl;

struct cell
{
    cell* left = nullptr;
    cell* right = nullptr;
    char* arr = nullptr;
    explicit cell(int len);
    void clearCell();
};

class word {
private:
    cell * head;
    cell * tail;
    int len = 1;
public:
    word(int l, string &str);
    word(const word& a);
    ~word();
    int findWord(word& str);
    friend ostream& operator << (ostream& os, word& str);
    int getLen();
    word& substr(int num, int count);
    word& operator= (word& a);
    word& operator= (string s);
    word& operator+ (word& a);
    word& operator+ (string a);
};

#endif //TL_WORD_H
