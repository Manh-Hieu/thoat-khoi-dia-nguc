$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $logPath

$count = 0
for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match "GenerateMinhSprites" -and $line -match "ColorFromHex") {
        Write-Output "Found potential matches in step $i"
        # Parse JSON and write the full content or tool_calls content to a scratch file
        $json = ConvertFrom-Json $line
        # Check tool_calls
        if ($json.tool_calls) {
            foreach ($tc in $json.tool_calls) {
                if ($tc.args.CodeContent) {
                    $c = $tc.args.CodeContent
                    $outPath = "scratch\minh_sprite_code_$count.txt"
                    $c | Out-File $outPath -Encoding utf8
                    Write-Output "Saved tool call CodeContent to $outPath (length: $($c.Length))"
                    $count++
                }
            }
        }
        # Check content
        if ($json.content) {
            $c = $json.content
            $outPath = "scratch\minh_sprite_content_$count.txt"
            $c | Out-File $outPath -Encoding utf8
            Write-Output "Saved step content to $outPath (length: $($c.Length))"
            $count++
        }
    }
}
