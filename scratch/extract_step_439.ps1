$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $logPath)[439]

$json = ConvertFrom-Json $line
if ($json.tool_calls) {
    $tc = $json.tool_calls[0]
    Write-Output "Tool: $($tc.name). Chunks count: $($tc.args.ReplacementChunks.Count)"
    $count = 0
    foreach ($chunk in $tc.args.ReplacementChunks) {
        $chunk.TargetContent | Out-File "scratch\step_439_chunk_${count}_target.txt" -Encoding utf8
        $chunk.ReplacementContent | Out-File "scratch\step_439_chunk_${count}_replacement.txt" -Encoding utf8
        Write-Output "Saved chunk $count target/replacement."
        $count++
    }
} else {
    Write-Output "No tool_calls in step 439."
}
