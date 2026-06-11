$logPath = "C:\Users\hieua\.gemini\antigravity\brain\98871e4d-8463-4de3-9b6e-e8e5d4ff6662\.system_generated\logs\transcript.jsonl"
$line = (Get-Content $logPath)[23]

# The JSON contains a "content" field with the file content
$json = ConvertFrom-Json $line
$fileContent = $json.content

# Let's find "GenerateMinhSprites()" in the fileContent and print the code block
$startIdx = $fileContent.IndexOf("private static void GenerateMinhSprites()")
if ($startIdx -ne -1) {
    # Let's find the closing brace or print about 4000 characters
    $sub = $fileContent.Substring($startIdx, 2500)
    Write-Output $sub
} else {
    Write-Output "GenerateMinhSprites method not found in line 23 content."
}
