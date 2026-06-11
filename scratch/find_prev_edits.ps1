$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $prevLogPath

$count = 0
for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    $json = ConvertFrom-Json $line
    if ($json.tool_calls) {
        foreach ($tc in $json.tool_calls) {
            $file = $tc.args.TargetFile
            if (!$file) { $file = $tc.args.AbsolutePath }
            if ($file -match "SpriteGenerator.cs") {
                Write-Output "Step $i`: name=$($tc.name), file=$file"
                # If it's a replacement, let's save the code!
                if ($tc.name -eq "replace_file_content" -or $tc.name -eq "write_to_file") {
                    $tc.args.CodeContent | Out-File "scratch\prev_edit_${i}_code.txt" -Encoding utf8
                    $tc.args.ReplacementContent | Out-File "scratch\prev_edit_${i}_replacement.txt" -Encoding utf8
                    Write-Output "Saved to scratch"
                }
            }
        }
    }
}
