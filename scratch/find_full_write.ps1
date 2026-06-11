$brainPath = "C:\Users\hieua\.gemini\antigravity\brain"
$folders = Get-ChildItem $brainPath -Directory

foreach ($folder in $folders) {
    $logPath = Join-Path $folder.FullName ".system_generated\logs\transcript.jsonl"
    if (Test-Path $logPath) {
        $lines = Get-Content $logPath
        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]
            if ($line -match "write_to_file" -and $line -match "SpriteGenerator\.cs") {
                Write-Output "Convo $($folder.Name) Step $i length: $($line.Length)"
                if ($line.Length -gt 10000) {
                    $json = ConvertFrom-Json $line
                    $c = $json.tool_calls[0].args.CodeContent
                    $c | Out-File "scratch\full_write_$($folder.Name)_$i.cs" -Encoding utf8
                    Write-Output "  Saved to scratch\full_write_$($folder.Name)_$i.cs"
                }
            }
        }
    }
}
