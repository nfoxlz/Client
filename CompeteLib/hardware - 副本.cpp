#include "pch.h"
#include "global.h"

#include <fstream>
#include <string>
#include <comdef.h>
#include <Wbemidl.h>

#pragma comment(lib, "wbemuuid.lib")

const char* queryWMI(const BSTR query, const LPCWSTR name)
{
    HRESULT hres;
    IWbemLocator* pLoc = NULL;
    IWbemServices* pSvc = NULL;
    IEnumWbemClassObject* pEnumerator = NULL;
    //hres = CoInitializeEx(0, COINIT_MULTITHREADED);
    //if (FAILED(hres))
    //    return nullptr;

    //hres = CoInitializeSecurity(NULL, -1, NULL, NULL, RPC_C_AUTHN_LEVEL_DEFAULT, RPC_C_IMP_LEVEL_IMPERSONATE, NULL, EOAC_NONE, NULL);
    //if (FAILED(hres))
    //{
    //    CoUninitialize();
    //    return nullptr;
    //}
    hres = CoCreateInstance(CLSID_WbemLocator, 0, CLSCTX_INPROC_SERVER, IID_IWbemLocator, (LPVOID*)&pLoc);
    if (FAILED(hres))
    {
        CoUninitialize();
        return nullptr;
    }
    hres = pLoc->ConnectServer(_bstr_t(L"ROOT\\CIMV2"), NULL, NULL, 0, NULL, 0, 0, &pSvc);
    if (FAILED(hres))
    {
        pLoc->Release();
        CoUninitialize();
        return nullptr;
    }
    // 设置请求的语言
    hres = CoSetProxyBlanket(
        pSvc,                        // 指向IWbemServices接口的指针
        RPC_C_AUTHN_WINNT,           // 认证方式
        RPC_C_AUTHZ_NONE,            // 授权方式
        NULL,                        // 服务Principal名称
        RPC_C_AUTHN_LEVEL_CALL,      // 认证等级
        RPC_C_IMP_LEVEL_IMPERSONATE, // 实现等级
        NULL,                        // 保留
        EOAC_NONE                    // 其他选项
    );
    if (FAILED(hres))
    {
        pLoc->Release();
        CoUninitialize();
        return nullptr;
    }
    hres = pSvc->ExecQuery(bstr_t("WQL"), query, WBEM_FLAG_FORWARD_ONLY | WBEM_FLAG_RETURN_IMMEDIATELY, NULL, &pEnumerator);
    if (FAILED(hres)) {
        pSvc->Release();
        pLoc->Release();
        CoUninitialize();
        return nullptr;
    }
    IWbemClassObject* pclsObj = NULL;
    ULONG uReturn = 0;
    BSTR result = SysAllocString(L"");;
    while (pEnumerator) {
        HRESULT hr = pEnumerator->Next(WBEM_INFINITE, 1, &pclsObj, /*&uReturn*/NULL);
        if (!uReturn)
            break;
        if (0 == hres) {
            VARIANT vtProp;
            hr = pclsObj->Get(name, 0, &vtProp, 0, 0);
            if (SUCCEEDED(hr))
            {
                //std::cout << "Motherboard Serial Number: " << vtProp.bstrVal << std::endl;
                VariantClear(&vtProp);
                result = vtProp.bstrVal;
            }
        }
        else
            break;
        pclsObj->Release();
    }
    pSvc->Release();
    pLoc->Release();
    pEnumerator->Release();
    CoUninitialize();

    return bstrToConstChar(result);
}


extern "C"
{
	__declspec(dllexport) const char* getCpuId()
	{
        return queryWMI(bstr_t("SELECT * FROM Win32_Processor"), L"ProcessorId");
    }

    __declspec(dllexport) const char* getMotherboardId()
    {
        return queryWMI(bstr_t("SELECT * FROM Win32_BaseBoard"), L"SerialNumber");
    }

    //__declspec(dllexport) DWORD getDiskId()
    //{
    //    WCHAR volumeName[MAX_PATH + 1] = { 0 };
    //    DWORD serialNumber = 0;
    //    DWORD maxComponentLen = 0;
    //    DWORD fileSystemFlags = 0;
    //    GetVolumeInformation(L"C:\\", volumeName, sizeof(volumeName), &serialNumber, &maxComponentLen, &fileSystemFlags, NULL, 0);
    //    return serialNumber;
    //}

    __declspec(dllexport) const char* getDiskId()
    {
        return queryWMI(bstr_t("SELECT * FROM Win32_DiskDrive"), L"SerialNumber");
    }
}