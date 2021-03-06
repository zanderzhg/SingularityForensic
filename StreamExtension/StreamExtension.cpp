// StreamExtension.cpp : Defines the exported functions for the DLL application.
//
//#include "StreanExtension.h"
#include "stdafx.h"
#include "../Contracts/Stream.h"
#include "StreamExtension.h"

using namespace std;

long long(*_getLengthFunc)();

long long(*_getPositionFunc)();

void(*_setPositionFunc)(long long pos);

bool(*_canReadFunc)();

int(*_readFunc)(BYTE *lpBuffer, int nNumberOfBytesToRead);

int(*_writeFunc)(BYTE* lpBuffer, int nNumberOfBytesToWrite);

//是否可写;
bool(*_canWriteFunc)();

long long UnManagedStream::GetLength() {
	if (_getLengthFunc == nullptr) {
		return 0;
	}

	return this->_getLengthFunc();
}

void UnManagedStream::SetGetLengthFunc(long long(*getLengthFunc)()) {
	this->_getLengthFunc = getLengthFunc;
}

//设定位置;(Seek);
void UnManagedStream::SetPosition(long long position) {
	if (_setPositionFunc == nullptr) {
		return;
	}

	_setPositionFunc(position);
}


//获取位置;
long long UnManagedStream:: GetPosition() {
	if (_getPositionFunc == nullptr) {
		return -1;
	}
	else {
		return _getPositionFunc();
	}
}

void UnManagedStream:: SetPositionFunc(long long(*getPositionFunc)(), void(*setPositionFunc)(long long pos)) {
	_getPositionFunc = getPositionFunc;
	_setPositionFunc = setPositionFunc;
}


void UnManagedStream:: SetCanReadFunc(bool(*canReadFunc)()) {
	_canReadFunc = canReadFunc;
}

bool UnManagedStream:: CanRead() {
	if (_canReadFunc == nullptr) {
		return false;
	}

	return _canReadFunc();
}

void UnManagedStream:: SetReadFunc(int(*readFunc)(BYTE *lpBuffer, int nNumberOfBytesToRead)) {
	_readFunc = readFunc;
}

//读取;
//参数lpBuffer:缓冲区;
//参数nNumberOfBytesToRead:读取大小;
//返回:实际读取大小;
bool UnManagedStream:: Read(BYTE * lpBuffer, unsigned long nNumberOfBytesToRead, unsigned long *nRetSize, long long nPos) {
	if (_readFunc == nullptr) {
		return false;
	}

	SetPosition(nPos);

	if (nNumberOfBytesToRead == 0) {
		return true;
	}

	auto readSize = _readFunc(lpBuffer, nNumberOfBytesToRead);

	if (nRetSize != nullptr) {
		*nRetSize = readSize;
	}

	return readSize != 0;
}

//int UnManagedStream:: Read(BYTE * lpBuffer, int nNumberOfBytesToRead, long long nPos) {
//
//	SetPosition(nPos);
//
//	return _readFunc(lpBuffer, nNumberOfBytesToRead);
//}
void UnManagedStream:: SetCanWriteFunc(bool(*canWriteFunc)()) {
	_canWriteFunc = canWriteFunc;
}

//是否可写;
bool UnManagedStream:: CanWrite() {
	if (_canWriteFunc == nullptr) {
		return false;
	}
}

void UnManagedStream:: SetWriteFunc(int(*writeFunc)(BYTE* lpBuffer, int nNumberOfBytesToWrite)) {
	_writeFunc = writeFunc;
}

//写入数据;
//参数lpBuffer:缓冲区;
//参数nNumberOfBytesToRead:写入大小;
//参数nPos:流的位置;
//返回:实际写入大小;
bool UnManagedStream:: Write(BYTE* lpBuffer, unsigned long nNumberOfBytesToWrite, unsigned long *nRetSize, long long nPos) {
	if (_writeFunc == nullptr) {
		return 0;
	}

	SetPosition(nPos);

	auto writeSize = _writeFunc(lpBuffer, nNumberOfBytesToWrite);

	if (nRetSize != nullptr) {
		*nRetSize = writeSize;
	}

	return writeSize != 0;
}

//关闭流;
void UnManagedStream:: Close() {
	_writeFunc = nullptr;
	_readFunc = nullptr;
	_canReadFunc = nullptr;
	_canWriteFunc = nullptr;
	_getPositionFunc = nullptr;
	_setPositionFunc = nullptr;
	_getLengthFunc = nullptr;
}
