[System.Reflection.Assembly]::LoadWithPartialName("System.Drawing") | Out-Null
$img = [System.Drawing.Image]::FromFile((Get-Item "Assets/Sprites/Characters/Minh/Minh_Idle_Down.png").FullName)
Write-Output "Dimensions: $($img.Width)x$($img.Height)"
$img.Dispose()
