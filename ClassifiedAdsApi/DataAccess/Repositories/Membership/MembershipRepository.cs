using DomainModel.Context;
using DomainModel.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Repositories.Membership;

public class MembershipRepository(ApplicationDbContext context) : IMembershipRepository
{
    public IQueryable<MembershipPlan> PlanQuery(bool tracking = false)
        => tracking ? context.MembershipPlans : context.MembershipPlans.AsNoTracking();

    public IQueryable<UserMembership> UserMembershipQuery(bool tracking = false)
        => tracking ? context.UserMemberships : context.UserMemberships.AsNoTracking();

    public Task<MembershipPlan?> GetPlanAsync(int id, bool tracking = true)
        => (tracking ? context.MembershipPlans : context.MembershipPlans.AsNoTracking())
            .FirstOrDefaultAsync(x => x.MembershipPlanID == id);

    public Task<UserMembership?> GetActiveAsync(string userId, bool tracking = true)
        => (tracking ? context.UserMemberships : context.UserMemberships.AsNoTracking())
            .Include(x => x.MembershipPlan)
            .Where(x => x.UserID == userId && x.IsActive)
            .OrderByDescending(x => x.StartDate)
            .FirstOrDefaultAsync();

    public Task AddPlanAsync(MembershipPlan plan)
        => context.MembershipPlans.AddAsync(plan).AsTask();

    public void RemovePlan(MembershipPlan plan) => context.MembershipPlans.Remove(plan);

    public Task AddUserMembershipAsync(UserMembership membership)
        => context.UserMemberships.AddAsync(membership).AsTask();

    public Task<List<UserMembership>> GetActiveMembershipsAsync(string userId)
        => context.UserMemberships.Where(x => x.UserID == userId && x.IsActive).ToListAsync();

    public async Task<bool> PlanHasUsageAsync(int planId)
        => await context.UserMemberships.AnyAsync(x => x.MembershipPlanID == planId)
           || await context.Payments.AnyAsync(x => x.MembershipPlanID == planId);

    public Task<int> SaveChangesAsync() => context.SaveChangesAsync();
}
