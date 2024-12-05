#include <iostream>

using namespace std;

#define BUFFER_SIZE 10

float buffer[BUFFER_SIZE]{};
int head = 0;
int tail = 0;
int asize = 0;

void insertIntoBuffer(float value, int& size, int& head, int& tail, float buffer[BUFFER_SIZE]) {
    if(size == BUFFER_SIZE) {
        head = (head + 1) % BUFFER_SIZE;
    }
    size++;
    buffer[tail] = value;
    tail = (tail + 1) % BUFFER_SIZE;
}

float extractFromBuffer(int& size, int& head, int& tail, float buffer[BUFFER_SIZE]) {
    if(size == 0) {
        return 0;
    }
    size--;
    float value = buffer[head];
    head = (head + 1) % BUFFER_SIZE;

    return value;
}

[[noreturn]] int main()
{
    while(1) {
        string input;
        cin >> input;

        if(input == "add") {
            float value;
            cin >> value;
            insertIntoBuffer(value, asize, head, tail, buffer);
        }
        else if(input == "extract") {
            float value = extractFromBuffer(asize, head, tail, buffer);
            cout << value << endl;
        }
        else if(input == "exit") {
            return 0;
        }

        cout << "head: " << head << endl;
        cout << "tail: " << tail << endl;
        for(float i : buffer) {
            cout << i << " ";
        }
        cout << endl;
    }
}
