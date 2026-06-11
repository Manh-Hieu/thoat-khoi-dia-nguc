$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
if (Test-Path $prevLogPath) {
    $lines = Get-Content $prevLogPath
    Write-Output "Previous log lines count: $($lines.Count)"
    
    # Search for SpriteGenerator.cs file views or updates
    $count = 0
    for ($i = 0; $i -lt $lines.Count; $i++) {
        $line = $lines[$i]
        if ($line -match "GenerateMinhSprites" -and $line -match "16, 32" -and $line -match "down =") {
            Write-Output "Found match in previous log step $i"
            # Extract content
            $json = ConvertFrom-Json $line
            $c = $json.content
            if (!$c) { $c = $json.tool_calls[0].args.CodeContent }
            if ($c) {
                $c | Out-File "scratch\prev_minh_sprite_code_$count.txt" -Encoding utf8
                Write-Output "Saved match to scratch\prev_minh_sprite_code_$count.txt (length: $($c.Length))"
                $count++
            }
        }
    }
    if ($count -eq 0) {
        # Try a broader search for just "GenerateMinhSprites"
        Write-Output "No match with 16, 32. Trying broader search..."
        for ($i = 0; $i -lt $lines.Count; $i++) {
            $line = $lines[$i]
            if ($line -match "GenerateMinhSprites" -and $line -match "down =") {
                Write-Output "Found match in previous log step $i"
                $json = ConvertFrom-Json $line
                $c = $json.content
                if (!$c) { $c = $json.tool_calls[0].args.CodeContent }
                if ($c) {
                    $c | Out-File "scratch\prev_minh_sprite_code_$count.txt" -Encoding utf8
                    Write-Output "Saved match to scratch\prev_minh_sprite_code_$count.txt (length: $($c.Length))"
                    $count++
                }
            }
        }
    }
} else {
    Write-Output "Previous log path does not exist."
}
