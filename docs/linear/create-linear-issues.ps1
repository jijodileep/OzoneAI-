<#
.SYNOPSIS
  Create OzoneAI backlog issues in Linear from ozoneai-backlog.csv

.NOTES
  Requires: $env:LINEAR_API_KEY
  Optional: $env:LINEAR_TEAM_ID (UUID). If missing, uses the first team from the API.
  Optional: $env:LINEAR_PROJECT_ID (UUID) to attach issues to project OzoneAI.
#>
$ErrorActionPreference = "Stop"
$apiKey = $env:LINEAR_API_KEY
if (-not $apiKey) {
  Write-Error "Set LINEAR_API_KEY first. Example: `$env:LINEAR_API_KEY='lin_api_...'"
}

$csvPath = Join-Path $PSScriptRoot "ozoneai-backlog.csv"
$rows = Import-Csv $csvPath

function Invoke-Linear {
  param(
    [string]$Query,
    [hashtable]$Variables = @{}
  )
  $payload = @{ query = $Query; variables = $Variables }
  $body = $payload | ConvertTo-Json -Depth 20 -Compress
  $headers = @{
    Authorization = $apiKey
    "Content-Type" = "application/json"
  }
  $resp = Invoke-RestMethod -Uri "https://api.linear.app/graphql" -Method Post -Headers $headers -Body $body
  if ($resp.errors) {
    throw ($resp.errors | ConvertTo-Json -Depth 5)
  }
  return $resp.data
}

# Resolve team
$teamId = $env:LINEAR_TEAM_ID
if (-not $teamId) {
  $data = Invoke-Linear -Query "query { teams { nodes { id name } } }"
  $team = $data.teams.nodes | Select-Object -First 1
  if (-not $team) { throw "No Linear teams found for this API key." }
  $teamId = $team.id
  Write-Host "Using team: $($team.name) ($teamId)"
}

# Resolve project OzoneAI if not provided
$projectId = $env:LINEAR_PROJECT_ID
if (-not $projectId) {
  $pdata = Invoke-Linear -Query "query { projects { nodes { id name } } }"
  $proj = $pdata.projects.nodes | Where-Object { $_.name -eq "OzoneAI" } | Select-Object -First 1
  if (-not $proj) {
    $proj = $pdata.projects.nodes | Where-Object { $_.name -like "*OzoneAI*" } | Select-Object -First 1
  }
  if ($proj) {
    $projectId = $proj.id
    Write-Host "Using project: $($proj.name) ($projectId)"
  }
  else {
    Write-Warning "Project OzoneAI not found; creating issues without project link."
  }
}

$priorityMap = @{ P0 = 2; P1 = 3; P2 = 4 } # Linear: 1 urgent, 2 high, 3 medium, 4 low

$created = 0
$failed = 0
foreach ($r in $rows) {
  $title = "[$($r.IssueKey)] $($r.Title)"
  $desc = @"
**Epic:** $($r.Epic)
**Issue key:** $($r.IssueKey)
**Team (planned):** $($r.Team)
**Spec:** $($r.SpecPath)

## Acceptance criteria
$($r.AcceptanceCriteria)

## Description
$($r.Description)

## Workflow
Branch: ``feat/$($r.IssueKey.ToLower().Replace('.','-'))-short-name``
PR must mention this issue title key for Linear-GitHub linking.
"@

  $prio = 3
  if ($priorityMap.ContainsKey($r.Priority)) { $prio = $priorityMap[$r.Priority] }

  $mutation = @'
mutation IssueCreate($input: IssueCreateInput!) {
  issueCreate(input: $input) {
    success
    issue { id identifier title url }
  }
}
'@

  $input = @{
    teamId = $teamId
    title = $title
    description = $desc
    priority = $prio
  }
  if ($projectId) {
    $input.projectId = $projectId
  }

  $vars = @{ input = $input }

  try {
    $result = Invoke-Linear -Query $mutation -Variables $vars
    if ($result.issueCreate.success) {
      Write-Host "Created $($result.issueCreate.issue.identifier) - $($result.issueCreate.issue.title)"
      $created++
    }
    else {
      Write-Warning "Failed $($r.IssueKey): issueCreate.success=false"
      $failed++
    }
  }
  catch {
    Write-Warning "Failed $($r.IssueKey): $_"
    $failed++
  }
  Start-Sleep -Milliseconds 250
}

Write-Host "Done. Created $created / $($rows.Count) issues. Failed: $failed"
