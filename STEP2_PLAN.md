# Step 2 Plan: Refine Persistence Layer and Migrations

Based on the completed Step 1 domain solidification, this step focuses on finalizing the persistence layer, validating migrations, and ensuring PostgreSQL compatibility. The domain models and initial migration `RefactorDomainForPortfolioAggregate` are already in place.

## Goals ✅
- Complete and validate EF Core configurations for the refined domain model
- Ensure all database constraints, indexes, and relationships are properly defined
- Validate PostgreSQL compatibility for all precision settings and constraints
- Add reference data seeding for activity types and other enumeration values
- Establish data integrity rules and cascade behaviors
- Create comprehensive database validation and health checks

## Workstreams

### 2.1 Complete EF Core Configuration Refinements

**Current State**: Basic entity configurations exist in `FolioDbContext.cs` but need refinement for production readiness.

**Tasks**:
- **Index Optimization** ✅ **IN PROGRESS**
  - Review and optimize existing indexes in each entity configuration method
  - Add performance-critical indexes for common query patterns:
    - `Activity.OccurredOn` for time-series queries
    - `Activity.Type` for filtering by activity type
    - `Activity.TransferGroupId` for transfer linkage queries
    - `AssetHistory.Date` and `AssetHistory.AssetId` composite index
    - `Asset.Symbol` and `Asset.Currency` composite index for lookups
  - Add unique constraints where needed (beyond existing `Asset.Symbol`)

- **Constraint Enhancement** ✅ **IN PROGRESS**
  - Add check constraints for currency codes (ISO 4217 format validation)
  - Add check constraints for positive amounts where applicable
  - Add check constraints for valid enum values in database
  - Add check constraints for FX rate positivity
  - Ensure `DisplayCurrency` constraints across Portfolio and Account

- **Relationship Refinement** ✅ **IN PROGRESS**
  - Validate and document all cascade delete behaviors
  - Review `ClientCascade` vs `Cascade` decisions for Asset relationships
  - Ensure soft-delete behavior doesn't conflict with foreign keys
  - Document transfer activity pairing invariants in configuration comments

- **Precision and Column Type Validation** ✅ **IN PROGRESS**  
  - Verify all `ConstValues` precision constants are applied correctly
  - Ensure consistent decimal precision across related fields
  - Validate PostgreSQL `numeric` type limits and performance implications
  - Add configuration comments documenting precision choices

### 2.2 Seed Data and Reference Data Management

**Current State**: No seed data mechanism exists.

**Tasks**:
- **Activity Type Seeding** ✅ **NOT STARTED**
  - Create `HasData()` configurations for standard `ActivityType` enum values
  - Ensure enum-to-database value mapping is explicit and stable
  - Document which activity types are system-provided vs user-defined

- **Currency Reference Data** ✅ **NOT STARTED**
  - Consider seeding common ISO 4217 currency codes for validation
  - Or implement validation logic to check against standard currency lists
  - Document currency validation approach (database constraint vs application validation)

- **Default User Setup** ✅ **NOT STARTED**
  - Ensure default user (`Guid.Empty`) is properly handled in configurations
  - Add documentation about single-tenant mode and `DefaultUserId` usage
  - Consider seeding a default user record for referential integrity

- **Sample Data for Development** ✅ **NOT STARTED**
  - Create optional development seed data (sample portfolios, accounts, activities)
  - Implement via separate migration or conditional seeding in `OnModelCreating`
  - Ensure sample data doesn't interfere with production deployments

### 2.3 Migration Validation and PostgreSQL Compatibility

**Current State**: `RefactorDomainForPortfolioAggregate` migration exists and builds successfully.

**Tasks**:
- **Migration Integrity Validation** ✅ **NOT STARTED**
  - Test migration up/down operations on clean database
  - Validate that all foreign key relationships work correctly
  - Test soft-delete scenarios don't break referential integrity
  - Ensure precision settings work correctly with PostgreSQL numeric types

- **PostgreSQL-Specific Features** ✅ **NOT STARTED**
  - Review use of PostgreSQL-specific features (timestamps, UUID generation)
  - Ensure `CURRENT_TIMESTAMP` default values work as expected
  - Validate date/time handling with `DateTimeOffset` types
  - Test performance of decimal precision settings under load

- **Data Type Compatibility** ✅ **NOT STARTED**
  - Validate that all EF Core types map correctly to PostgreSQL
  - Test edge cases for decimal precision and rounding
  - Ensure currency string lengths are appropriate
  - Validate UUID performance and indexing

- **Migration Scripting and Deployment** ✅ **NOT STARTED**
  - Create SQL script generation for production deployment
  - Document migration rollback procedures
  - Test migrations against different PostgreSQL versions (if applicable)
  - Ensure migrations work with the containerized setup in `ci-cd/docker-compose.yml`

### 2.4 Database Health Checks and Validation

**Current State**: No database health check mechanism exists.

**Tasks**:
- **Connection and Schema Validation** ✅ **NOT STARTED**
  - Implement health check endpoint to validate database connectivity
  - Add schema validation to ensure all expected tables and constraints exist
  - Create diagnostic queries to validate data integrity rules

- **Business Rule Validation** ✅ **NOT STARTED**
  - Add database-level validation for portfolio-account relationships
  - Validate activity transfer group integrity
  - Ensure soft-delete cascading works as expected
  - Add validation for currency consistency within portfolios

- **Performance Baseline** ✅ **NOT STARTED**
  - Document expected query performance for common operations
  - Validate index effectiveness for portfolio/account/activity queries
  - Test performance with sample data volumes
  - Document recommended PostgreSQL configuration settings

### 2.5 Data Access Pattern Documentation

**Current State**: Basic `IDbContextFactory<FolioDbContext>` setup exists but patterns not documented.

**Tasks**:
- **Context Lifecycle Management** ✅ **NOT STARTED**
  - Document proper usage of `IDbContextFactory<FolioDbContext>`
  - Provide examples of scoped context usage for transactions
  - Document when to use factory vs direct injection patterns

- **Transaction Boundary Guidelines** ✅ **NOT STARTED**
  - Document transaction patterns for multi-entity operations
  - Provide examples for activity posting, portfolio creation workflows
  - Document rollback and error handling patterns

- **Query Pattern Examples** ✅ **NOT STARTED**
  - Document efficient query patterns for common scenarios
  - Provide examples of proper Include() usage for navigation properties
  - Document soft-delete filtering patterns

## Deliverables

### Primary Deliverables
1. **Enhanced EF Core Configuration** - Complete entity configurations with optimized indexes, constraints, and relationships
2. **Validated Migration Set** - Working migration that can be applied cleanly to PostgreSQL with proper rollback support
3. **Seed Data Infrastructure** - Mechanism for seeding reference data and optional development data
4. **Database Health Checks** - Health check endpoint and validation queries for production monitoring
5. **Data Access Documentation** - Comprehensive guide for using the persistence layer correctly

### Configuration Files
- `backend/Zuricos.Folio.Data/FolioDbContext.cs` - Enhanced with complete configurations
- `backend/Zuricos.Folio.Migrations.Psql/` - Validated and tested migrations
- `backend/Zuricos.Folio.Api/Setup/SetupServices.cs` - Enhanced with health checks
- `documentation/data-access-patterns.md` - Data access guidelines and examples

### Testing Artifacts
- SQL scripts for migration testing
- Sample data generation scripts
- Performance validation queries
- Database schema validation procedures

## Technical Requirements

### Database Constraints
- All monetary fields use appropriate precision (19,4 for money, 18,8 for FX rates, 20,8 for quantities)
- Currency fields limited to 3 characters (ISO 4217)
- Positive amount constraints where applicable
- Valid enum value constraints in database

### Index Strategy
- Primary performance indexes for time-series queries on activities
- Composite indexes for common filtering patterns
- Unique constraints for business rules (Symbol uniqueness)
- Foreign key indexes for relationship traversal

### PostgreSQL Compatibility
- All features compatible with PostgreSQL 13+
- Proper handling of UUID generation and indexing
- Correct timestamp handling with timezone support
- Optimal numeric precision configuration

## Dependencies and Prerequisites

### External Dependencies
- PostgreSQL 13+ running and accessible
- Existing domain models from Step 1 (✅ Complete)
- Current migration `RefactorDomainForPortfolioAggregate` (✅ Complete)

### Internal Dependencies
- All Step 1 domain model work must be complete (✅ Complete)
- Build system must be working (✅ Verified)
- Connection string and configuration must be valid (✅ Verified)

## Risk Mitigation

### Data Integrity Risks
- **Risk**: Soft deletes could break referential integrity
- **Mitigation**: Comprehensive testing of cascade behaviors and constraint validation

### Performance Risks  
- **Risk**: Decimal precision settings could impact query performance
- **Mitigation**: Performance testing with realistic data volumes and index optimization

### Migration Risks
- **Risk**: Complex migration could fail in production
- **Mitigation**: Thorough testing of up/down migrations, SQL script generation for manual deployment

### Compatibility Risks
- **Risk**: PostgreSQL-specific features might not work as expected
- **Mitigation**: Explicit testing against target PostgreSQL version, documentation of version requirements

## Success Criteria

1. **Build Success**: All projects build without warnings or errors
2. **Migration Success**: Migrations can be applied cleanly to empty database and rolled back without issues
3. **Data Integrity**: All business rules are enforced at database level with appropriate constraints
4. **Performance Baseline**: Query performance meets acceptable thresholds for common operations
5. **Documentation Complete**: Data access patterns and migration procedures are fully documented
6. **Health Check Functional**: Database health check endpoint validates schema and connectivity
7. **Seed Data Working**: Reference data seeding works correctly without conflicts

## Next Steps Preparation

This step prepares for Step 3 (Application Services) by ensuring:
- Stable and validated persistence layer
- Clear patterns for data access and transaction management
- Comprehensive understanding of database performance characteristics  
- Reliable migration and deployment procedures
- Foundation for implementing business logic services with confidence

## Timeline Estimate

- **Workstream 2.1** (EF Core Configuration): 2-3 days
- **Workstream 2.2** (Seed Data): 1-2 days  
- **Workstream 2.3** (Migration Validation): 1-2 days
- **Workstream 2.4** (Health Checks): 1 day
- **Workstream 2.5** (Documentation): 1 day

**Total Estimated Duration**: 6-9 days

Individual workstreams can be executed in parallel where dependencies allow, with 2.1 being the primary prerequisite for other workstreams.