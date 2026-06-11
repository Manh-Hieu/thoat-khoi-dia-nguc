$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"

$steps = @(413, 478)
foreach ($s in $steps) {
    if ($s -lt (Get-Content $prevLogPath).Count) {
        $line = (Get-Content $prevLogPath)[$s]
        $json = ConvertFrom-Json $line
        Write-Output "Step $s tool: $($json.tool_calls[0].name)"
        
        # Extract ReplacementChunks or TargetContent/ReplacementContent
        if ($json.tool_calls) {
            $tc = $json.tool_calls[0]
            if ($tc.name -eq "multi_replace_file_content") {
                $count = 0
                foreach ($chunk in $tc.args.ReplacementChunks) {
                    $chunk.ReplacementContent | Out-File "scratch\prev_step_${s}_chunk_${count}_r.txt" -Encoding utf8
                    Write-Output "Saved chunk $count of step $s."
                    $count++
                }
            } elseif ($tc.name -eq "replace_file_content") {
                $tc.args.ReplacementContent | Out-File "scratch\prev_step_${s}_r.txt" -Encoding utf8
                Write-Output "Saved step $s replacement content."
            }
        }
    }
}
