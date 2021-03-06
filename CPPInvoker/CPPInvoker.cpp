// CPPInvoker.cpp : Defines the entry point for the console application.
//
#pragma once
#include "stdafx.h"
#include "../StreamExtension/DevStream.h"
#include "../Contracts/Stream.h"
#include <Windows.h>
HANDLE MOpenFile(LPCWSTR lpFileName, DWORD dwDesiredAccess = GENERIC_READ) {
	//#define FILE_SHARE_READ                 0x00000001
	//#define FILE_SHARE_WRITE                0x00000002  
	//#define OPEN_EXISTING       3
	//#define FILE_ATTRIBUTE_NORMAL               0x00000080  
	//#define GENERIC_READ                     (0x80000000L)
	_tprintf_s(L"MCreateFile:We got %ls", lpFileName);
	return CreateFile(lpFileName, dwDesiredAccess, FILE_SHARE_READ | FILE_SHARE_WRITE, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL);
}


int main()
{
	
	//auto fileSz = sizeof(fi);
	auto handle = MOpenFile(_T("D:\\1.E01"));
	DevStream* devStream = new DevStream(handle);
	BYTE s[512];
	unsigned long retSize = 0;
	devStream->Read(s, 512, &retSize, 0);
    return 0;
}

