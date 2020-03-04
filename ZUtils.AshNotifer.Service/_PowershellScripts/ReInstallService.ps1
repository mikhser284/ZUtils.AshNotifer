#Powershell v7

$MyServiceName = 'ZUtils.AshNotiferService'
$MyServicePath = 'C:\_AppPublish\_AshNotiferService\ZUtils.AshNotifer.Service.exe'
Clear-Host;
$ServiceExist = $false

if(Get-Service $MyServiceName -ErrorAction SilentlyContinue)
{
    $ServiceExist = $true
}
if($ServiceExist)
{
    Write-Output 'Service already exist'
    Write-Output 'Stoping and deleting existing service ...'
    $myService = Get-Service $MyServiceName;
    $myService | Format-List;
    
    if($myService.Status -ne 'Stopped')
    {
        Write-Output "Stopping service ...";
        $myService.Stop();
        $maxRepeat = 120
        do
        {
            $myService = Get-Service $MyServiceName;
            $maxRepeat--
            Start-Sleep -Milliseconds 250
        } while (-not($myService.Status -eq 'Stopped' -or $maxRepeat -le 0))    
        Write-Output 'Service ' $myService.Status
    }
    if($myService.Status -eq 'Stopped')
    {
        Write-Output "Removing service ...";
        Remove-Service $myService.Name
    }
    else
    {
        Write-Output 'Operation failed'    
    }
}

if(Get-Service $MyServiceName -ErrorAction SilentlyContinue)
{
    $ServiceExist = $true
}
if(-not $ServiceExist)
{
    Write-Output 'Service not exist'
    Write-Output 'Installing service ...'
    New-Service -Name $MyServiceName -BinaryPathName $MyServicePath
    Write-Output 'Running service ...'
    Start-Service -Name $MyServiceName
}
