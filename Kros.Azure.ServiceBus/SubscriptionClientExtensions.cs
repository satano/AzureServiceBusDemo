using Microsoft.Azure.ServiceBus;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Kros.Azure.ServiceBus
{
    public static class SubscriptionClientExtensions
    {
        // Do not modify properties of this instance.
        private static readonly RuleDescription _defaultRule = new RuleDescription();

        public static async Task<bool> ContainsRuleAsync(this SubscriptionClient client, string ruleName)
            => (await client.GetRulesAsync()).Any(rule => rule.Name.Equals(ruleName, StringComparison.OrdinalIgnoreCase));

        public static async Task RemoveRuleIfExistsAsync(this SubscriptionClient client, string ruleName)
        {
            if (await client.ContainsRuleAsync(ruleName))
            {
                await client.RemoveRuleAsync(ruleName);
            }
        }

        public static async Task RemoveDefaultRuleAsync(this SubscriptionClient client)
            => await client.RemoveRuleIfExistsAsync(RuleDescription.DefaultRuleName);

        public static async Task AddDefaultRuleAsync(this SubscriptionClient client)
            => await client.AddRuleIfNotExistsAsync(_defaultRule);

        public static async Task AddRuleIfNotExistsAsync(this SubscriptionClient client, RuleDescription rule)
        {
            if (!await client.ContainsRuleAsync(rule.Name))
            {
                await client.AddRuleAsync(rule);
            }
        }
    }
}
