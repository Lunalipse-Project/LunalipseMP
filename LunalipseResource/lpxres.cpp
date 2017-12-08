// LunalipseResource.cpp : 定义 DLL 应用程序的导出函数。
//

#include "stdafx.h"
#include "lpxr.h"

LXPRESOURCE::LXPRESOURCE()
{
}

LXPRESOURCE::~LXPRESOURCE()
{
	vector<resource>::iterator v;
	for (v = res.begin(); v != res.end(); v++)
	{
		//释放指向的内存空间
		free(&v);
	}
	res.clear();
}

rExport* LXPRESOURCE::rgain(int index)
{
	if (res.size() == 0)return NULL;
	resource r = res[index];
	rExport* re = new rExport();
	re->dpointer = (&r)->dpointer;
	strcpy(re->ft, (&r)->ftype);
	return re;
}

int LXPRESOURCE::rload(char* path)
{
	rheader* r = (rheader*)malloc(sizeof(rheader));
	spnote* sn = (spnote*)malloc(sizeof(spnote));
	dblock* db = (dblock*)malloc(sizeof(dblock));
	FILE* f = fopen(path,"w");
	if (f != NULL)
	{
		//读入资源文件头
		fread(r, sizeof(rheader), 1, f);
		int totalf = r->fcount;
		char sig[16];
		strcpy(sig, r->sig);
		//校验幻数
		if (r->magic != MAGIC)return -1;
		free(r);
		for (int i = 0; i < totalf; i++)
		{
			fread(sn, sizeof(spnote), 1, f);
			byte* combined = (byte*)malloc(sn->flen);
			int tkb = sn->bcount;
			for (int j = 0; j < tkb; j++)
			{
				fread(db, sizeof(dblock), 1, f);
				memcpy(combined + 1025 * j, db->dat, 1024);
			}
			resource r;
			r.dpointer = combined;
			strcpy(r.ftype, *sn->ftype);
			res.push_back(r);
		}
		free(db);
		free(sn);
		fclose(f);
	}
	else
	{
		MessageBox(NULL, TEXT("Unable to load the resource file"), TEXT("Error"), MB_OK);
		return -2;
	}
	return res.size();
}

int LXPRESOURCE::rsearch(char* ftype)
{
	for (int i=0;i<res.size();i++)
	{
		if (strcmp(ftype, (&res[i])->ftype) == 0)
		{
			return i;
		}
	}
}

