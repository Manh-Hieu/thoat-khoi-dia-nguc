$brainPath = "C:\Users\hieua\.gemini\antigravity\brain"
$folders = Get-ChildItem $brainPath -Directory

foreach ($folder in $folders) {
    $logPath = Join-Path $folder.FullName ".system_generated\logs\transcript.jsonl"
    if (Test-Path $logPath) {
        $lines = Get-Content $logPath
        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]
            if ($line -match "Minh_Walk_Down" -and $line -match "16" -and $line -match "32" -and $line -match "colorMap") {
                Write-Output "Found MATCH in Convo $($folder.Name) Step $i (line length $($line.Length))!"
                # Save it to scratch
                $line | Out-File "scratch\found_1632_relaxed_$($folder.Name)_$i.txt" -Encoding utf8
            }
        }
    }
}
