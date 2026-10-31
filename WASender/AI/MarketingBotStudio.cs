using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WASender.AI
{
    public partial class MarketingBotStudio : Form
    {
        private readonly ListBox _availableSteps;
        private readonly FlowLayoutPanel _botCanvas;
        private readonly TextBox _messageInput;
        private readonly TextBox _botPreview;
        private readonly Button _runDemoButton;

        private readonly string[] _stepTemplates =
        {
            "Lead qualification",
            "Order intake",
            "Appointment booking",
            "Follow-up reminder",
            "Human handoff"
        };

        public MarketingBotStudio()
        {
            Text = "AI Marketing Bot Studio";
            Width = 960;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);

            var leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 260,
                BackColor = Color.FromArgb(245, 246, 250),
                Padding = new Padding(12)
            };

            var title = new Label
            {
                Text = "Bot components",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 8)
            };

            _availableSteps = new ListBox
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 10F),
                BorderStyle = BorderStyle.FixedSingle,
                Height = 240,
                Margin = new Padding(0, 0, 0, 10)
            };

            _availableSteps.Items.AddRange(_stepTemplates);
            _availableSteps.SelectionMode = SelectionMode.One;
            _availableSteps.MouseDown += OnAvailableStepMouseDown;

            var builderLabel = new Label
            {
                Text = "Drag workflow cards into the bot canvas.",
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 12, 0, 8)
            };

            var canvasPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12)
            };

            var flowTitle = new Label
            {
                Text = "Conversational flow",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 8)
            };

            _botCanvas = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(10),
                AllowDrop = true,
                MinimumSize = new Size(0, 200)
            };

            _botCanvas.DragEnter += BotCanvas_DragEnter;
            _botCanvas.DragDrop += BotCanvas_DragDrop;

            var inputPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 160,
                Padding = new Padding(12)
            };

            var inputLabel = new Label
            {
                Text = "Customer message",
                AutoSize = true,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 6)
            };

            _messageInput = new TextBox
            {
                Dock = DockStyle.Top,
                Multiline = true,
                Height = 70,
                BorderStyle = BorderStyle.FixedSingle,
                Text = "Hi, I’d like to schedule a product demo and learn about pricing."
            };

            _runDemoButton = new Button
            {
                Text = "Run AI flow demo",
                Dock = DockStyle.Top,
                Height = 36,
                Margin = new Padding(0, 12, 0, 0)
            };
            _runDemoButton.Click += RunDemoButton_Click;

            _botPreview = new TextBox
            {
                Dock = DockStyle.Fill,
                Multiline = true,
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            leftPanel.Controls.Add(_availableSteps);
            leftPanel.Controls.Add(builderLabel);
            leftPanel.Controls.Add(title);

            canvasPanel.Controls.Add(_botCanvas);
            canvasPanel.Controls.Add(flowTitle);

            inputPanel.Controls.Add(_botPreview);
            inputPanel.Controls.Add(_runDemoButton);
            inputPanel.Controls.Add(_messageInput);
            inputPanel.Controls.Add(inputLabel);

            Controls.Add(leftPanel);
            Controls.Add(canvasPanel);
            Controls.Add(inputPanel);

            SetDefaultBotFlow();
        }

        private void OnAvailableStepMouseDown(object sender, MouseEventArgs e)
        {
            if (_availableSteps.SelectedItem == null)
            {
                return;
            }

            var selected = _availableSteps.SelectedItem.ToString();
            _availableSteps.DoDragDrop(selected, DragDropEffects.Move);
        }

        private void BotCanvas_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void BotCanvas_DragDrop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                return;
            }

            var stepName = e.Data.GetData(DataFormats.StringFormat).ToString();
            AddStepCard(stepName);
        }

        private void SetDefaultBotFlow()
        {
            AddStepCard("Lead qualification");
            AddStepCard("Appointment booking");
            AddStepCard("Human handoff");
        }

        private void AddStepCard(string stepName)
        {
            var card = new Label
            {
                Text = stepName,
                Width = 180,
                Height = 42,
                BorderStyle = BorderStyle.FixedSingle,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleCenter,
                Margin = new Padding(0, 0, 0, 8),
                BackColor = Color.FromArgb(234, 245, 255),
                ForeColor = Color.FromArgb(20, 60, 100),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold)
            };

            card.MouseDown += (sender, args) =>
            {
                if (args.Button == MouseButtons.Left)
                {
                    card.DoDragDrop(stepName, DragDropEffects.Move);
                }
            };

            _botCanvas.Controls.Add(card);
        }

        private void RunDemoButton_Click(object sender, EventArgs e)
        {
            var userMessage = _messageInput.Text;
            var intent = MarketingConversationEngine.ClassifyIntent(userMessage);
            var definition = MarketingConversationEngine.BuildBotDefinition(intent);

            _botCanvas.Controls.Clear();
            if (definition != null && definition.Steps != null)
            {
                foreach (var step in definition.Steps)
                {
                    AddStepCard(step.Name);
                }
            }

            _botPreview.Text = MarketingConversationEngine.GenerateResponse(userMessage);
        }
    }
}
