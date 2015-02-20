#pragma once
#include <string.h>
#include <windows.h>

#pragma data_seg(".SHARE")
HWND hAppWnd = NULL;
HHOOK hHook;
HINSTANCE hDllModule;
#pragma data_seg()
#pragma comment(linker, "/section:.SHARE,rws")

enum BorderSkinMsgs
{
	Activated = WM_APP + 1,
	TitleChanged = WM_SETTEXT,
	IconChanged = WM_APP + 3,
	Closed = WM_APP + 4,
	Maximized = WM_APP + 5,
	SizeChanged = WM_APP + 6,
	LocationChanged = WM_APP + 7,
	ManualUpdate = WM_APP + 8,
	DeActivated = WM_APP + 9,
	VisibleChanged = WM_APP + 10,
	EnabledChanged = WM_APP + 11,
	SizeMoveBegins = WM_APP + 12,
	SizeMoveEnds = WM_APP + 13,
	SysCommandDrag = WM_APP + 14,
	WindowDetected = WM_APP + 15
};

const int BorderRemove = 1;

bool CheckWindow(HWND hWnd);
bool HasSkinWindow(HWND hWnd);
void RestoreBorder(HWND hWnd);
bool CALLBACK EnumWindowsProc(HWND hwnd,LPARAM lParam);
LRESULT NotifyApp(HWND hWnd, BorderSkinMsgs Msg, LPARAM lParam = NULL);
extern "C" __declspec(dllexport) LRESULT CALLBACK CallWndProc(int nCode,WPARAM wParam,LPARAM lParam);
extern "C" __declspec(dllexport) LRESULT CALLBACK CallWndProcRet(int nCode,WPARAM wParam,LPARAM lParam);
extern "C" __declspec(dllexport) int StopWindowsHook(void);

BOOL WINAPI DllMain( HINSTANCE hModule, unsigned long  ul_reason_for_call,void* lpReserved )
{
	hDllModule = hModule;
	switch (ul_reason_for_call)
	{
		case DLL_PROCESS_ATTACH:
			 break;
		case DLL_THREAD_ATTACH:
			 break;
		case DLL_THREAD_DETACH:
			 break;
		case DLL_PROCESS_DETACH:
			if (hAppWnd != NULL && !IsWindow(hAppWnd))
			{
				EnumWindows((WNDENUMPROC)EnumWindowsProc,NULL);
				StopWindowsHook();
				hAppWnd = NULL;
			}
			break;
    }
    return TRUE;
}

bool CALLBACK EnumWindowsProc(HWND hWnd,LPARAM lParam)
{
	//(IsWindowVisible(hWnd) | IsIconic(hWnd))
	if (CheckWindow(hWnd) && HasSkinWindow(hWnd))
	{
		RestoreBorder(hWnd);
	}
	return true;
}

extern "C" __declspec(dllexport) bool StartWindowsHook(HWND hWnd)
{
	if (hHook == NULL)
	{
		hAppWnd = hWnd;
		hHook = SetWindowsHookEx(WH_CALLWNDPROC,HOOKPROC(CallWndProc),hDllModule,0); 

		if (hHook != NULL)
			return true;
	}
	return false;
}

extern "C" __declspec(dllexport) int StopWindowsHook(void)
{
	int result = 0;
	if (hHook != NULL)
	{
		result = UnhookWindowsHookEx(hHook);
		if (result)
			hHook = NULL;
	}
	return result;
}

bool CheckWindow(HWND hWnd)
{
	long Style = GetWindowLong(hWnd, GWL_STYLE);
	long ExStyle = GetWindowLong(hWnd, GWL_EXSTYLE);
	HWND Parent = GetParent(hWnd);

	if ((Parent != HWND_MESSAGE) && !(Style & WS_CHILD) && !(ExStyle & WS_EX_TOOLWINDOW))// && IsWindowVisible(hWnd))
	{
		if ( ((Style & WS_CAPTION) && (ExStyle & WS_EX_WINDOWEDGE)) || 
			 (Style & WS_MAXIMIZEBOX) )
			return true;
	}
	return false;
}

LRESULT NotifyApp(HWND hWnd, BorderSkinMsgs Msg, LPARAM lParam)
{
	LRESULT NotifyAppReturn = 0;
	SendMessageTimeout(hAppWnd, Msg, WPARAM(hWnd), lParam,
		SMTO_ABORTIFHUNG | SMTO_NORMAL, 100, (PDWORD_PTR)&NotifyAppReturn);
	return NotifyAppReturn;
}

void ReAdjustMaximizeSize(HWND hWnd)
{
	HMONITOR hMoniter = MonitorFromWindow(hWnd,MONITOR_DEFAULTTONEAREST);

	MONITORINFO *MoniterInfo = new MONITORINFO();
	MoniterInfo->cbSize = sizeof(MONITORINFO);
	GetMonitorInfo(hMoniter,MoniterInfo);

	SetWindowPos(hWnd, 0,MoniterInfo->rcWork.left, 
						 MoniterInfo->rcWork.top,
						 MoniterInfo->rcWork.right - MoniterInfo->rcWork.left,
						 MoniterInfo->rcWork.bottom - MoniterInfo->rcWork.top,
						 SWP_NOACTIVATE | SWP_NOOWNERZORDER | SWP_NOZORDER);
	delete MoniterInfo;
}

void RemoveBorder(HWND hWnd,int Width, int Height, LRESULT Result)
{
	HRGN rgn;
	int CaptionHeight = GetSystemMetrics(SM_CYCAPTION);
	int FrameMetrics = GetSystemMetrics(SM_CXFIXEDFRAME);

	if ((GetWindowLong(hWnd, GWL_STYLE) & WS_SIZEBOX) == WS_SIZEBOX)
		FrameMetrics = GetSystemMetrics(SM_CXFRAME);

	FrameMetrics += GetSystemMetrics(SM_CXPADDEDBORDER);
	
	if (IsZoomed(hWnd) | IsIconic(hWnd))
	{
		FrameMetrics = 0;
		CaptionHeight = 0;
	}

	if (Result != BorderRemove)
		CaptionHeight += (int)Result;

	rgn = CreateRectRgn	(FrameMetrics,
						 CaptionHeight + FrameMetrics,
						 Width -  FrameMetrics,
						 Height - FrameMetrics);

	SetWindowRgn(hWnd,rgn,true);
}

bool HasSkinWindow(HWND hWnd)
{
	if ((IsZoomed(hWnd) && (GetWindowLong(hWnd,GWL_STYLE) & WS_POPUP)))
		return true;

	HRGN hRgn; LPRECT Rect = new RECT;
	bool result = false;

	hRgn = CreateRectRgn(0,0,0,0);
	GetWindowRgn(hWnd,hRgn);
	if (GetRgnBox(hRgn,Rect) == SIMPLEREGION && Rect->left > 0 && Rect->top > 0)
		result = true;

	delete Rect;
	DeleteObject(hRgn);
	return result;
}

void RestoreBorder(HWND hWnd)
{
	if ((IsZoomed(hWnd) && (GetWindowLong(hWnd,GWL_STYLE) & WS_POPUP)) | IsIconic(hWnd))
	{
		SetWindowLong(hWnd,GWL_STYLE,
			GetWindowLong(hWnd,GWL_STYLE) & ~WS_POPUP | WS_CAPTION | WS_BORDER | WS_SIZEBOX );
		
		SetWindowLong(hWnd,GWL_EXSTYLE,
			GetWindowLong(hWnd,GWL_EXSTYLE) & ~WS_EX_STATICEDGE | WS_EX_WINDOWEDGE );
		
		ReAdjustMaximizeSize(hWnd);
	}
	SetWindowRgn(hWnd,NULL,true);
	PostMessage(hWnd,WM_THEMECHANGED, NULL, NULL);
	SetWindowPos(hWnd, 0, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE |
									  SWP_NOZORDER | SWP_NOREPOSITION | SWP_FRAMECHANGED );
	UpdateWindow(hWnd);
}

LRESULT CALLBACK CallWndProc(int nCode,WPARAM wParam,LPARAM lParam)
{

	if( (lParam!=NULL) && (nCode == HC_ACTION))
	{
		CWPSTRUCT *CwpStruct = (CWPSTRUCT *) lParam;

		if (!CheckWindow(CwpStruct->hwnd))
			return CallNextHookEx(hHook,nCode,wParam,lParam);

		switch(CwpStruct->message)
		{
				case WM_ACTIVATE:
				if ( LOWORD(CwpStruct->wParam) == WA_INACTIVE )
					NotifyApp(CwpStruct->hwnd,DeActivated,CwpStruct->lParam);
				else
					NotifyApp(CwpStruct->hwnd,Activated,CwpStruct->lParam);
				break;

			case WM_NCDESTROY:
				NotifyApp(CwpStruct->hwnd,Closed);
				break;

			case WM_SETTEXT:
				NotifyApp(CwpStruct->hwnd,TitleChanged,CwpStruct->lParam);
				break;
				
			case WM_SETICON:
				if (CwpStruct->wParam == ICON_SMALL)
					NotifyApp(CwpStruct->hwnd,IconChanged,CwpStruct->lParam);
				break;
				
			case WM_ENTERSIZEMOVE:
				NotifyApp(CwpStruct->hwnd,SizeMoveBegins);
				break;
				
			case WM_EXITSIZEMOVE:
				NotifyApp(CwpStruct->hwnd,SizeMoveEnds);
				break;

			case WM_ENABLE:
				NotifyApp(CwpStruct->hwnd,EnabledChanged,CwpStruct->wParam);
				break;

			case WM_MOVE:{
				// TODO: REALLY NEEDS REFACTORING, THIS EVENTS REALLY EXCUTES MAXIMIZED EVEnT WHICH IS NOT WRONG
				NotifyApp(CwpStruct->hwnd,Maximized,true);
				break;
			}

			case WM_SHOWWINDOW:
				NotifyApp(CwpStruct->hwnd,VisibleChanged,CwpStruct->wParam);
				break;

			//case WM_WINDOWPOSCHANGED:
				//{
				//WINDOWPOS *WindowPos = (WINDOWPOS *) CwpStruct->lParam;
				//
				//}
				//break;

			case WM_WINDOWPOSCHANGING:
				{
				WINDOWPOS *WindowPos = (WINDOWPOS *) CwpStruct->lParam;
				if (!(WindowPos->flags & SWP_NOREDRAW))
				{
					if ((WindowPos->flags & SWP_SHOWWINDOW) == SWP_SHOWWINDOW)
						NotifyApp(CwpStruct->hwnd,VisibleChanged,true);
					
					if ((WindowPos->flags & SWP_HIDEWINDOW) == SWP_HIDEWINDOW)
						NotifyApp(CwpStruct->hwnd,VisibleChanged,false);

					if (!(WindowPos->flags & SWP_NOMOVE))
						NotifyApp(CwpStruct->hwnd,LocationChanged,MAKELONG(WindowPos->x,WindowPos->y));
					
					if (!(WindowPos->flags & SWP_NOSIZE))
					{
						LRESULT Return = (LRESULT)NotifyApp(CwpStruct->hwnd,SizeChanged,MAKELONG(WindowPos->cx,WindowPos->cy));
						if (Return > 0)
							RemoveBorder(CwpStruct->hwnd, WindowPos->cx, WindowPos->cy, Return);
					}

					if (!(WindowPos->flags & SWP_NOSIZE) || !(WindowPos->flags & SWP_NOMOVE))
						NotifyApp(CwpStruct->hwnd,ManualUpdate);
					}
				}
				break;
				
			case WM_SYSCOMMAND:
				if (CwpStruct->wParam == 61458) // 61458 is SC_DRAGMOVE
					NotifyApp(CwpStruct->hwnd,SysCommandDrag,WPARAM(CwpStruct->wParam));
				break;
				
			//case WM_NCCALCSIZE: // has many problems
			//case WM_NCPAINT:	// well good choice but still slow
			//case WM_SIZING:	// well good choice but still slow
			//	NotifyApp(CwpStruct->hwnd,WindowDetected);
			//	break;
		}
	}
	return CallNextHookEx(hHook,nCode,wParam,lParam);
}