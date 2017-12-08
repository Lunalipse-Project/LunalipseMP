#pragma once
#define MAGIC 0x4C55

#include <stdio.h>
#include <vector>

using namespace std;

typedef unsigned char byte;
struct rheader
{
	public int magic;
	int fcount;
	char sig[16];
};
struct spnote
{
	int index;
	long flen;
	int bcount;
	char* ftype[8];
};
struct dblock
{
	int bindex;
	byte* dat[1024];
};
struct resource
{
	char ftype[8];
	byte* dpointer;
};
struct rExport
{
	char* ft;
	byte* dpointer;
};

class LXPRESOURCE
{
public:
	LXPRESOURCE();
	~LXPRESOURCE();
	int rload(char* path);
	int rsearch(char* ftype);
	rExport* rgain(int index);
private:
	vector<resource> res;
};