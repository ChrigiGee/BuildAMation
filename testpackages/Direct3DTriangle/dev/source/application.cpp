#include "application.h"
#include "renderer.h"
#include "errorhandler.h"
#include "common.h"

#include <Windows.h>
#include <string>

std::string ssWindowClassName("Test Window Class");
::HWND shMainWindow = 0;

static ::LRESULT CALLBACK WindowProc(::HWND hWnd, ::UINT Msg, ::WPARAM wParam, ::LPARAM lParam)
{
    Application *application = Application::GetInstance();
    switch (Msg)
    {
    case WM_CREATE:
        {
            Renderer *renderer = new Renderer(hWnd);
            renderer->Initialize();
            application->SetRenderer(renderer);
        }
        break;

    case WM_DESTROY:
        {
            Renderer *renderer = application->GetRenderer();
            if (renderer != 0)
            {
                renderer->Release();
                delete renderer;
                application->SetRenderer(0);
            }
        }
        break;

    case WM_CLOSE:
        {
            Renderer *renderer = application->GetRenderer();
            if (0 != renderer)
            {
                renderer->Exit();
            }

            REPORTERROR("Sending quit message");
            ::PostQuitMessage(0);
        }
        return 0;
    }

    return ::DefWindowProc(hWnd, Msg, wParam, lParam);
}

Application *Application::spInstance = 0;
Application *Application::GetInstance()
{
    return spInstance;
}

Application::Application(int UNUSEDARG(argc), char *UNUSEDARG(argv)[])
: mpRenderer(0), mhWin32Instance(0), mi32ExitCode(0)
{
    if (0 != spInstance)
    {
        REPORTERROR("There is already an instance of the application running");
        return;
    }

    spInstance = this;
}

void Application::SetWin32Instance(void *instance)
{
    mhWin32Instance = instance;
}

int Application::Run()
{
    this->RegisterWindowClass();
    this->CreateMainWindow();
    this->MainLoop();
    this->DestroyMainWindow();
    this->UnregisterWindowClass();
    return this->mi32ExitCode;
}

void Application::RegisterWindowClass()
{
    ::WNDCLASSEX windowClass;
    ::ZeroMemory(&windowClass, sizeof(windowClass));
    windowClass.cbSize = sizeof(windowClass);
    windowClass.hInstance = static_cast< ::HINSTANCE>(this->mhWin32Instance);
    windowClass.lpfnWndProc = WindowProc;
    windowClass.lpszClassName = ssWindowClassName.c_str();
    UINT style = CS_HREDRAW | CS_VREDRAW | CS_OWNDC;
    windowClass.style = style;
    windowClass.hbrBackground = reinterpret_cast<HBRUSH>(COLOR_BACKGROUND);

    ::ATOM returnValue = ::RegisterClassEx(&windowClass);
    if (0 == returnValue)
    {
        ::DWORD error = ::GetLastError();
        REPORTWIN32ERROR("Failed to register class; error %d '%s'", error);
        return;
    }

    return;
}

void Application::UnregisterWindowClass()
{
    ::BOOL result = ::UnregisterClass(ssWindowClassName.c_str(), static_cast< ::HINSTANCE>(this->mhWin32Instance));
    if (0 == result)
    {
        ::DWORD error = ::GetLastError();
        REPORTWIN32ERROR("Failed to unregister class; error %d, '%s'", error);
        return;
    }
}

void Application::CreateMainWindow()
{
    DWORD exStyle = 0;
    std::string mainWindowName("Direct3D triangle rendering test");
    DWORD style = WS_CLIPSIBLINGS | WS_CLIPCHILDREN | WS_OVERLAPPEDWINDOW;
    int x = CW_USEDEFAULT;
    int y = CW_USEDEFAULT;
    int width = 512;
    int height = 512;
    ::HWND parentWindow = 0;
    ::HMENU menuHandle = 0;
    ::LPVOID lpParam = 0;

    shMainWindow = ::CreateWindowEx(
        exStyle,
        ssWindowClassName.c_str(),
        mainWindowName.c_str(),
        style,
        x,
        y,
        width,
        height,
        parentWindow,
        menuHandle,
        static_cast< ::HINSTANCE>(this->mhWin32Instance),
        lpParam);
    if (0 == shMainWindow)
    {
        ::DWORD error = ::GetLastError();
        REPORTWIN32ERROR("Failed to create window; error %d, '%s'", error);
        return;
    }

    ::UpdateWindow(shMainWindow);
    ::ShowWindow(shMainWindow, SW_SHOWDEFAULT);
}

void Application::DestroyMainWindow()
{
    if (0 != shMainWindow && ::IsWindow(shMainWindow))
    {
        BOOL result = ::DestroyWindow(shMainWindow);
        if (0 == result)
        {
            ::DWORD error = ::GetLastError();
            REPORTWIN32ERROR("Failed to destroy window; error %d, '%s'", error);
            return;
        }
    }
}

void Application::MainLoop()
{
    ::MSG msg;

    // loop until WM_QUIT(0) received
    while(::GetMessage(&msg, 0, 0, 0) > 0)
    {
        ::TranslateMessage(&msg);
        ::DispatchMessage(&msg);
    }

    this->mi32ExitCode = (int)msg.wParam;
}

void Application::SetRenderer(Renderer *renderer)
{
    this->mpRenderer = renderer;
}

Renderer *Application::GetRenderer()
{
    return this->mpRenderer;
}
