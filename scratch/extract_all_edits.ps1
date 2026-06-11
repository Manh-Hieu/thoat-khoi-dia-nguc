$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $logPath

$steps = @(141, 173, 394, 439)
foreach ($s in $steps) {
    if ($s -lt $lines.Count) {
        $line = $lines[$s]
        $json = ConvertFrom-Json $line
        if ($json.tool_calls) {
            $tc = $json.tool_calls[0]
            Write-Output "Step $s tool: $($tc.name)"
            if ($tc.name -eq "replace_file_content") {
                $tc.args.TargetContent | Out-File "scratch\step_${s}_target.txt" -Encoding utf8
                $tc.args.ReplacementContent | Out-File "scratch\step_${s}_replacement.txt" -Encoding utf8
                Write-Output "Saved step $s target/replacement."
            } elseif ($tc.name -eq "multi_replace_file_content") {
                # Save each chunk target/replacement
                $count = 0
                foreach ($chunk in $tc.args.ReplacementChunks) {
                    $chunk.TargetContent | Out-File "scratch\step_${s}_chunk_${count}_target.txt" -Encoding utf8
                    $chunk.ReplacementContent | Out-File "scratch\step_${s}_chunk_${count}_replacement.txt" -Encoding utf8
                    Write-Output "Saved step $s chunk $count target/replacement."
                    $count++
                }
            }
        }
    }
}
