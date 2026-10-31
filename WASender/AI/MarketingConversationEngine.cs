using System;
using System.Collections.Generic;
using System.Linq;

namespace WASender.AI
{
    public enum MarketingBotIntent
    {
        LeadQualification,
        OrderManagement,
        AppointmentBooking,
        FollowUp,
        Unknown
    }

    public sealed class MarketingBotStep
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Prompt { get; set; }
    }

    public sealed class MarketingBotDefinition
    {
        public string Name { get; set; }
        public MarketingBotIntent Intent { get; set; }
        public IList<MarketingBotStep> Steps { get; set; }
    }

    public static class MarketingConversationEngine
    {
        public static MarketingBotIntent ClassifyIntent(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return MarketingBotIntent.Unknown;
            }

            var message = userMessage.Trim();
            var lowered = message.ToLowerInvariant();

            if (ContainsAny(lowered, "order", "buy", "purchase", "checkout", "invoice", "price", "shipping", "product"))
            {
                return MarketingBotIntent.OrderManagement;
            }

            if (ContainsAny(lowered, "appointment", "book", "schedule", "meeting", "demo", "reservation", "visit"))
            {
                return MarketingBotIntent.AppointmentBooking;
            }

            if (ContainsAny(lowered, "lead", "interested", "hello", "contact", "sales", "qualify", "follow up", "question", "motivation"))
            {
                return MarketingBotIntent.LeadQualification;
            }

            if (ContainsAny(lowered, "reminder", "followup", "follow up", "after sales", "status"))
            {
                return MarketingBotIntent.FollowUp;
            }

            return MarketingBotIntent.Unknown;
        }

        public static MarketingBotDefinition BuildBotDefinition(MarketingBotIntent intent)
        {
            switch (intent)
            {
                case MarketingBotIntent.LeadQualification:
                    return new MarketingBotDefinition
                    {
                        Name = "Lead Qualification Bot",
                        Intent = intent,
                        Steps = new List<MarketingBotStep>
                        {
                            new MarketingBotStep { Name = "Warm greeting", Description = "Welcome and confirm the customer intent.", Prompt = "Hi! Thanks for reaching out. Are you looking for product details or a quick consultation?" },
                            new MarketingBotStep { Name = "Need discovery", Description = "Ask for budget, urgency, and target outcome.", Prompt = "Great. What are you trying to achieve, and when do you need it?" },
                            new MarketingBotStep { Name = "Qualification handoff", Description = "Route qualified leads to sales or follow-up.", Prompt = "Perfect. I’m forwarding your profile to our sales team for the next best step." },
                            new MarketingBotStep { Name = "CRM update", Description = "Persist lead status and notes.", Prompt = "Lead status: qualified, follow-up scheduled, and sales notes captured." }
                        }
                    };

                case MarketingBotIntent.OrderManagement:
                    return new MarketingBotDefinition
                    {
                        Name = "Order Management Bot",
                        Intent = intent,
                        Steps = new List<MarketingBotStep>
                        {
                            new MarketingBotStep { Name = "Order intake", Description = "Capture order intent and item details.", Prompt = "I can help you place or check an order. What product would you like?" },
                            new MarketingBotStep { Name = "Payment confirmation", Description = "Confirm quantity, price, and payment method.", Prompt = "Let me confirm the total, preferred payment option, and delivery preferences." },
                            new MarketingBotStep { Name = "Order status", Description = "Keep the customer updated throughout fulfillment.", Prompt = "Your order is in progress. I can send updates on confirmation, shipping, and delivery." },
                            new MarketingBotStep { Name = "Support escalation", Description = "Escalate exceptions or failed payments.", Prompt = "I’ll route this to the order support team for a quick resolution." }
                        }
                    };

                case MarketingBotIntent.AppointmentBooking:
                    return new MarketingBotDefinition
                    {
                        Name = "Appointment Booking Bot",
                        Intent = intent,
                        Steps = new List<MarketingBotStep>
                        {
                            new MarketingBotStep { Name = "Availability check", Description = "Find open slots and confirm time preferences.", Prompt = "I can help schedule a meeting. Which day and time works best for you?" },
                            new MarketingBotStep { Name = "Booking confirmation", Description = "Collect contact details and meeting objective.", Prompt = "Thanks. I’ll confirm your preferred meeting type and contact details." },
                            new MarketingBotStep { Name = "Reminder", Description = "Send a reminder before the call or visit.", Prompt = "A reminder will be sent before your appointment to keep everything on track." },
                            new MarketingBotStep { Name = "Follow-up", Description = "Capture the outcome and schedule a next step.", Prompt = "After the appointment, we’ll share the recap and any next action items." }
                        }
                    };

                case MarketingBotIntent.FollowUp:
                    return new MarketingBotDefinition
                    {
                        Name = "Follow-Up Bot",
                        Intent = intent,
                        Steps = new List<MarketingBotStep>
                        {
                            new MarketingBotStep { Name = "Status check", Description = "Review previous conversation and pending needs.", Prompt = "Just checking in — is everything still on track?" },
                            new MarketingBotStep { Name = "Customer update", Description = "Ask if the customer needs changes, rescheduling, or more information.", Prompt = "Would you like to update the order, reschedule, or get more details?" },
                            new MarketingBotStep { Name = "Close the loop", Description = "Document the final result and next step.", Prompt = "Thanks for the update. I’ve logged the latest status and the next action." }
                        }
                    };

                default:
                    return new MarketingBotDefinition
                    {
                        Name = "AI Marketing Assistant",
                        Intent = MarketingBotIntent.Unknown,
                        Steps = new List<MarketingBotStep>
                        {
                            new MarketingBotStep { Name = "Intent detection", Description = "Identify the primary customer goal.", Prompt = "I can help with lead qualification, product orders, or appointment scheduling." },
                            new MarketingBotStep { Name = "Smart routing", Description = "Route the conversation to the proper operational flow.", Prompt = "I’ll direct this to the right workflow based on your request." },
                            new MarketingBotStep { Name = "Human handoff", Description = "Send unresolved issues to a human agent.", Prompt = "If you’d like, I can connect you with a sales or support specialist." }
                        }
                    };
            }
        }

        public static string GenerateResponse(string userMessage)
        {
            var intent = ClassifyIntent(userMessage);
            var definition = BuildBotDefinition(intent);

            if (definition == null || definition.Steps == null || definition.Steps.Count == 0)
            {
                return "I can assist with lead qualification, order management, appointment booking, or follow-up tasks.";
            }

            var steps = string.Join(", ", definition.Steps.Select(s => s.Name));
            return string.Format("AI intent: {0}. Suggested flow: {1}.", definition.Intent, steps);
        }

        private static bool ContainsAny(string text, params string[] values)
        {
            if (string.IsNullOrEmpty(text) || values == null || values.Length == 0)
            {
                return false;
            }

            return values.Any(value => !string.IsNullOrEmpty(value) && text.Contains(value, StringComparison.OrdinalIgnoreCase));
        }
    }
}
