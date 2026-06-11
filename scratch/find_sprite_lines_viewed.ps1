$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $logPath

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match "SpriteGenerator\.cs") {
        $json = ConvertFrom-Json $line
        if ($json.type -eq "VIEW_FILE") {
            Write-Output "Step $i is VIEW_FILE for SpriteGenerator.cs"
            # Print arguments
            if ($json.tool_calls -and $json.tool_calls.Count -gt 0) {
                $tc = $json.tool_calls[0]
                Write-Output "  StartLine: $($tc.args.StartLine), EndLine: $($tc.args.EndLine)"
            }
        }
    }
}
