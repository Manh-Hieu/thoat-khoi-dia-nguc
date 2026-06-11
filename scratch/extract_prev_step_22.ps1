$prevLogPath = "C:\Users\hieua\.gemini\antigravity\brain\738071b7-3a6f-4959-95f9-4dab128e550b\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $prevLogPath)[22]

$json = ConvertFrom-Json $line
$c = $json.content
if (!$c -and $json.tool_calls) {
    $c = $json.tool_calls[0].args.CodeContent
}
if ($c) {
    $c | Out-File "scratch\prev_original_sprite_generator.cs" -Encoding utf8
    Write-Output "Saved previous step 22 content to scratch\prev_original_sprite_generator.cs"
} else {
    Write-Output "Checking step 23..."
    $line23 = (Get-Content $prevLogPath)[23]
    $json23 = ConvertFrom-Json $line23
    if ($json23.content) {
        $json23.content | Out-File "scratch\prev_original_sprite_generator.cs" -Encoding utf8
        Write-Output "Saved previous step 23 content to scratch\prev_original_sprite_generator.cs"
    } else {
        Write-Output "Step 23 has no content either."
    }
}
