$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $logPath)[412]

$json = ConvertFrom-Json $line
$c = $json.content
if ($c) {
    $c | Out-File "scratch\original_1632_generator.cs" -Encoding utf8
    Write-Output "Saved step 412 content to scratch\original_1632_generator.cs (length: $($c.Length))"
} else {
    Write-Output "No content in step 412."
}
