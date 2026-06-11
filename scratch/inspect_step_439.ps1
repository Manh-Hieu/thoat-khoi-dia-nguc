$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $logPath)[439]

Write-Output "Step 439 JSON length: $($line.Length)"
$json = ConvertFrom-Json $line
if ($json.tool_calls) {
    $tc = $json.tool_calls[0]
    Write-Output "Tool name: $($tc.name)"
    Write-Output "Args properties: $($tc.args | Get-Member -MemberType NoteProperty | Select-Object -ExpandProperty Name)"
    Write-Output "ReplacementChunks class: $($tc.args.ReplacementChunks.GetType().Name)"
    Write-Output "Chunks count: $($tc.args.ReplacementChunks.Count)"
    if ($tc.args.ReplacementChunks.Count -gt 0) {
        $c = $tc.args.ReplacementChunks[0]
        Write-Output "Chunk 0 TargetContent length: $($c.TargetContent.Length)"
        Write-Output "Chunk 0 ReplacementContent length: $($c.ReplacementContent.Length)"
        
        # Write to temp files using alternative way to ensure it is written correctly
        [System.IO.File]::WriteAllText("scratch\step_439_t.txt", $c.TargetContent)
        [System.IO.File]::WriteAllText("scratch\step_439_r.txt", $c.ReplacementContent)
        Write-Output "Saved to scratch\step_439_t.txt and scratch\step_439_r.txt"
    }
}
