using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WASender.MarketingAI
{
    public enum MarketingIntentType
    {
        LeadQualification,
        OrderManagement,
        AppointmentBooking,
        Fallback
    }

    public class MarketingLeadContext
    {
        public string LeadName { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Industry { get; set; } = "";
        public string Interest { get; set; } = "";
        public string Source { get; set; } = "";
        public bool Qualified { get; set; }
        public string QualificationNotes { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class MarketingOrderContext
    {
        public string CustomerName { get; set; } = "";
        public string Product { get; set; } = "";
        public string Quantity { get; set; } = "";
        public decimal Price { get; set; }
        public string Status { get; set; } = "Pending";
        public string Notes { get; set; } = "";
    }

    public class MarketingAppointmentContext
    {
        public string CustomerName { get; set; } = "";
        public string PreferredDate { get; set; } = "";
        public string PreferredTime { get; set; } = "";
        public string Purpose { get; set; } = "";
        public string Status { get; set; } = "Pending";
    }

    public class MarketingConversationState
    {
        public MarketingIntentType Intent { get; set; }
        public string CurrentStep { get; set; } = "start";
        public string UserMessage { get; set; } = "";
        public MarketingLeadContext Lead { get; set; } = new MarketingLeadContext();
        public MarketingOrderContext Order { get; set; } = new MarketingOrderContext();
        public MarketingAppointmentContext Appointment { get; set; } = new MarketingAppointmentContext();
        public List<string> History { get; set; } = new List<string>();
    }

    public class ClaudeMarketingService
    {
        public string ApiKey { get; set; } = "";

        public MarketingIntentType ClassifyIntent(string userMessage, string currentContext = "")
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return MarketingIntentType.Fallback;
            }

            var normalized = userMessage.ToLowerInvariant();
            if (normalized.Contains("order") || normalized.Contains("buy") || normalized.Contains("price") || normalized.Contains("quote"))
            {
                return MarketingIntentType.OrderManagement;
            }

            if (normalized.Contains("appointment") || normalized.Contains("book") || normalized.Contains("schedule") || normalized.Contains("call") || normalized.Contains("meeting"))
            {
                return MarketingIntentType.AppointmentBooking;
            }

            if (normalized.Contains("hello") || normalized.Contains("hi") || normalized.Contains("interested") || normalized.Contains("need") || normalized.Contains("want") || normalized.Contains("details"))
            {
                return MarketingIntentType.LeadQualification;
            }

            return MarketingIntentType.Fallback;
        }

        public string BuildQualificationPrompt(string userMessage, MarketingLeadContext lead)
        {
            return $@"You are a sales qualification bot. Determine whether this customer is a good prospect.
Context:
- Name: {lead.LeadName}
- Interest: {lead.Interest}
- Industry: {lead.Industry}
- Source: {lead.Source}
Customer message: {userMessage}
Return a short next-step recommendation and whether the lead should be qualified or follow-up later.";
        }

        public string BuildOrderPrompt(string userMessage, MarketingOrderContext order)
        {
            return $@"You are an order assistance bot. Capture order intent and summarize what the customer wants.
Order context:
- Product: {order.Product}
- Quantity: {order.Quantity}
- Price: {order.Price}
Customer message: {userMessage}
Return structured order notes and next step.";
        }

        public string BuildAppointmentPrompt(string userMessage, MarketingAppointmentContext appointment)
        {
            return $@"You are an appointment scheduler. Capture the meeting purpose and preferred booking details.
Context:
- Name: {appointment.CustomerName}
- Preferred date: {appointment.PreferredDate}
- Preferred time: {appointment.PreferredTime}
Customer message: {userMessage}
Return the next booking message and any missing details.";
        }

        public string GetRecommendedNextAction(MarketingConversationState state)
        {
            if (state == null)
            {
                return "Start the conversation by asking what the customer is looking for.";
            }

            switch (state.Intent)
            {
                case MarketingIntentType.LeadQualification:
                    return "Ask for the customer need, budget range, and preferred timeline, then qualify the lead.";
                case MarketingIntentType.OrderManagement:
                    return "Confirm the product, quantity, price, and delivery expectations before sending a final order summary.";
                case MarketingIntentType.AppointmentBooking:
                    return "Confirm the preferred date and time, then send a meeting confirmation.";
                default:
                    return "Reply with a helpful fallback message and offer to help with product, pricing, or scheduling.";
            }
        }
    }

    public class MarketingBotStudio : Form
    {
        private readonly RichTextBox _conversationBox;
        private readonly TextBox _inputBox;
        private readonly ListBox _flowList;
        private readonly Button _startBtn;
        private readonly Button _qualifyBtn;
        private readonly Button _orderBtn;
        private readonly Button _appointmentBtn;
        private readonly ClaudeMarketingService _service;

        public MarketingBotStudio()
        {
            this.Text = "AI Marketing Bot Studio";
            this.Size = new Size(900, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            _service = new ClaudeMarketingService();

            _conversationBox = new RichTextBox
            {
                ReadOnly = true,
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };

            _inputBox = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = 90,
                Multiline = true,
                Font = new Font("Segoe UI", 10)
            };

            _flowList = new ListBox
            {
                Dock = DockStyle.Left,
                Width = 220,
                Font = new Font("Segoe UI", 10)
            };

            _startBtn = new Button { Text = "Lead Qualification", Dock = DockStyle.Top, Height = 42 };
            _qualifyBtn = new Button { Text = "Run Qualify Flow", Dock = DockStyle.Top, Height = 42 };
            _orderBtn = new Button { Text = "Run Order Flow", Dock = DockStyle.Top, Height = 42 };
            _appointmentBtn = new Button { Text = "Run Booking Flow", Dock = DockStyle.Top, Height = 42 };

            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                Width = 220,
                FlowDirection = FlowDirection.TopDown,
                AutoSize = true,
                WrapContents = false
            };

            buttonPanel.Controls.Add(_startBtn);
            buttonPanel.Controls.Add(_qualifyBtn);
            buttonPanel.Controls.Add(_orderBtn);
            buttonPanel.Controls.Add(_appointmentBtn);

            _startBtn.Click += (s, e) => RunFlow(MarketingIntentType.LeadQualification);
            _qualifyBtn.Click += (s, e) => RunFlow(MarketingIntentType.LeadQualification);
            _orderBtn.Click += (s, e) => RunFlow(MarketingIntentType.OrderManagement);
            _appointmentBtn.Click += (s, e) => RunFlow(MarketingIntentType.AppointmentBooking);

            _flowList.Items.Add("Lead Qualification");
            _flowList.Items.Add("Order Capture");
            _flowList.Items.Add("Appointment Booking");
            _flowList.Items.Add("Fallback Reply");

            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Panel1MinSize = 220,
                Panel2MinSize = 260
            };

            split.Panel1.Controls.Add(_flowList);

            var mainContent = new Panel { Dock = DockStyle.Fill };
            mainContent.Controls.Add(_conversationBox);
            mainContent.Controls.Add(_inputBox);
            _conversationBox.Height = 400;
            _inputBox.Top = _conversationBox.Bottom;
            _inputBox.Height = 90;
            _inputBox.Dock = DockStyle.Bottom;

            split.Panel2.Controls.Add(mainContent);

            var host = new Panel { Dock = DockStyle.Fill };
            host.Controls.Add(split);
            host.Controls.Add(buttonPanel);
            buttonPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;

            this.Controls.Add(host);

            _inputBox.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter && !e.Shift)
                {
                    e.SuppressKeyPress = true;
                    HandleMessage(_inputBox.Text);
                    _inputBox.Clear();
                }
            };

            AppendMessage("AI Marketing Bot Studio ready. Start with a customer message or trigger a bot flow.");
        }

        private void RunFlow(MarketingIntentType intent)
        {
            var sample = intent switch
            {
                MarketingIntentType.LeadQualification => "Hello, I am looking for a better solution for my sales process. Can you help?",
                MarketingIntentType.OrderManagement => "I want to buy 3 units of your starter package. What is the price?",
                MarketingIntentType.AppointmentBooking => "Can we book a demo call for next Tuesday at 3 PM?",
                _ => "Hi, I need support with my order or booking."
            };

            AppendMessage($"[System] Running flow: {intent}");
            HandleMessage(sample);
        }

        private void HandleMessage(string userMessage)
        {
            if (string.IsNullOrWhiteSpace(userMessage))
            {
                return;
            }

            var state = new MarketingConversationState
            {
                UserMessage = userMessage,
                Intent = _service.ClassifyIntent(userMessage),
                History = new List<string> { userMessage }
            };

            var recommendation = _service.GetRecommendedNextAction(state);
            AppendMessage($"Customer: {userMessage}");
            AppendMessage($"AI: {recommendation}");
        }

        private void AppendMessage(string message)
        {
            _conversationBox.AppendText(message + Environment.NewLine + Environment.NewLine);
            _conversationBox.SelectionStart = _conversationBox.TextLength;
            _conversationBox.ScrollToCaret();
        }
    }
}
