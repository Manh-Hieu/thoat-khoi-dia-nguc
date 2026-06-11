$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
$lines = Get-Content $prevLogPath

for ($i = 0; $i -lt $lines.Count; $i++) {
    $line = $lines[$i]
    if ($line -match "SpriteGenerator\.cs" -and $line -match '"type":"WRITE_TO_FILE"') {
        Write-Output "Step $i is WRITE_TO_FILE for SpriteGenerator.cs"
        # Extract the content to scratch
        $json = ConvertFrom-Json $line
        $c = $json.tool_calls[0].args.CodeContent
        if ($c) {
            $c | Out-File "scratch\prev_created_sprite_generator.cs" -Encoding utf8
            Write-Output "Saved to scratch\prev_created_sprite_generator.cs (length: $($c.Length))"
        }
    }
}
