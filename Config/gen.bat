@echo off
chcp 65001 >nul

set WORKSPACE=..
set LUBAN_DLL=%WORKSPACE%\Tools\Luban\Luban.dll
set CONF_ROOT=.

@REM Assets目录
set ASSETS_DIR=%WORKSPACE%\Assets


@REM *********输出目标*********
set TARGET_MAIN=client
set OUTPUT_CODE_DIR=Scripts\Config\LubanGen
set OUTPUT_DATA_DIR=Resources\Configs\LubanData\bin


@REM 调用生成函数
call :GenerateLubanConfig %TARGET_MAIN% "%OUTPUT_CODE_DIR%" "%OUTPUT_DATA_DIR%"

pause
exit /b 0

@REM *************** 函数定义区域 ***************

@REM ========================================
@REM 函数：生成Luban配置
@REM 参数1：输出代码目录
@REM 参数2：输出数据目录  
@REM 参数3：配置根目录（可选，默认值为"."）
@REM ========================================
:GenerateLubanConfig
    setlocal
    set sTarget=%~1
    set sOutputCodeDir=%~2
    set sOutputDataDir=%~3

    @REM 如果配置根目录为空，使用默认值
    if "%sConfRoot%"=="" set sConfRoot=%CONF_ROOT%

    echo ========================================
    echo 正在生成Luban配置...
    echo 输出代码目录: %sOutputCodeDir%
    echo 输出数据目录: %sOutputDataDir%
    echo ========================================
    echo.

    dotnet %LUBAN_DLL% ^
        -t %sTarget% ^
        -d bin ^
        -c cs-bin ^
        --conf %CONF_ROOT%\luban.conf ^
        -x outputCodeDir=%ASSETS_DIR%\%sOutputCodeDir% ^
        -x outputDataDir=%ASSETS_DIR%\%sOutputDataDir%

    echo.
    echo 生成完成！
    endlocal
exit /b