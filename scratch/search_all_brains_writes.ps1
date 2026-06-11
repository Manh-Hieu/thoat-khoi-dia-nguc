$brainPath = "C:\Users\hieua\.gemini\antigravity\brain"
$folders = Get-ChildItem $brainPath -Directory

foreach ($folder in $folders) {
    $logPath = Join-Path $folder.FullName ".system_generated\logs\transcript.jsonl"
    if (Test-Path $logPath) {
        $lines = Get-Content $logPath
        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]
            if ($line -match "SpriteGenerator\.cs" -and ($line -match "replace_file_content" -or $line -match "write_to_file" -or $line -match "multi_replace_file_content")) {
                # This step edited SpriteGenerator.cs!
                # Let's check if it contains Minh_Idle_Down
                if ($line -match "Minh_Idle_Down") {
                    Write-Output "Found WRITE MATCH in Convo $($folder.Name) Step $i (line length $($line.Length))!"
                    # Extract the CodeContent or ReplacementContent or chunks directly
                    $json = ConvertFrom-Json $line
                    if ($json.tool_calls) {
                        $tc = $json.tool_calls[0]
                        if ($tc.name -eq "write_to_file") {
                            $tc.args.CodeContent | Out-File "scratch\found_write_file_$($folder.Name)_$i.txt" -Encoding utf8
                        } elseif ($tc.name -eq "replace_file_content") {
                            $tc.args.ReplacementContent | Out-File "scratch\found_replace_$($folder.Name)_$i.txt" -Encoding utf8
                        } elseif ($tc.name -eq "multi_replace_file_content") {
                            $tc.args.ReplacementChunks[0].ReplacementContent | Out-File "scratch\found_multi_$($folder.Name)_$i.txt" -Encoding utf8
                        }
                    }
                }
            }
        }
    }
}
