# Refresh Token Cleanup Implementation Plan

## Overview
This document outlines the implementation plan for automated cleanup of expired and revoked refresh tokens using Hangfire to maintain database.

## Solution: Hangfire-Based Cleanup

### Why Hangfire?
- **Reliability**: Jobs persist across application restarts
- **Scalability**: Works seamlessly across multiple server instances
- **Monitoring**: Built-in dashboard for job tracking and analytics
- **Resilience**: Automatic retry logic for failed jobs
- **Flexibility**: Supports complex scheduling patterns

## Implementation Steps

### Step 1: Add Hangfire NuGet Package
### Step 2: Create TokenCleanupService
### Step 3: Register Hangfire
### Step 4: Schedule Recurring Job
### Step 5: Add Database Indexes
For optimal query performance during cleanup:
```sql
CREATE INDEX idx_refresh_tokens_expires_at 
ON auth."RefreshTokens"("ExpiresAt");

CREATE INDEX idx_refresh_tokens_revoked_expires_at 
ON auth."RefreshTokens"("IsRevoked", "ExpiresAt");
```

## Hangfire Dashboard Access

Monitor:
- Job execution history
- Success/failure rates
- Execution duration
- Retry attempts

## Configuration Options

### Scheduling Frequency
- Daily at 2 AM or once a week on weekend
- Can adjust via `Cron` expressions
- off-peak hours for execution 

### Cleanup Retention Policy
- Proposition: 30-day grace period for revoked tokens
- Adjust `AddDays(-30)` in cleanup logic
- Consider regulatory/audit requirements
- Document retention policy

### Job Timeout

## Monitoring & Alerts

### Metrics to Track
- Tokens deleted per execution
- Cleanup job duration (in ms)
- Failed cleanup attempts
- Database table size over time

### Alert Thresholds (Nice to have)
- Job failure: Alert on 3+ consecutive failures
- Duration: Alert if cleanup takes > 5 minutes
- Table size: Alert if tokens table grows > 10% 

### Job Retention
- Hangfire stores job history in database
- Configure retention in job storage options
- Default: Keep 1 week of completed jobs

## Security Considerations
- Restrict Hangfire dashboard access to authenticated admins
- Implement IP whitelisting for dashboard access
- Log all cleanup operations for audit purposes
- Consider encrypting Hangfire database storage

## Rollback Strategy
If cleanup causes performance issues:
1. Disable Hangfire job via dashboard
2. Revert to manual cleanup via admin endpoint
3. Analyze slow queries and add/optimize indexes
4. Resume scheduled cleanup with adjusted frequency

## Future Enhancements
- Add cleanup metrics to application health checks
- Implement configurable cleanup schedules per environment
- Create dashboard widget for token cleanup status
- Integrate with centralized monitoring (DataDog, New Relic, etc.)