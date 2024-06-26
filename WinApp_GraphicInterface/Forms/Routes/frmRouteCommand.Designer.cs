﻿
namespace WinApp_GraphicInterface.Forms.Routes
{
    partial class frmRouteCommand
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listView2 = new System.Windows.Forms.ListView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listView1 = new System.Windows.Forms.ListView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAreaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.pnlMainDrowing = new System.Windows.Forms.Panel();
            this.pnlCanvas = new System.Windows.Forms.Panel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.fromPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toPointToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelAndReturnHomeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startRouteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.robotCommandToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.areaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlRulerLeft = new System.Windows.Forms.Panel();
            this.pnlLeft = new System.Windows.Forms.Panel();
            this.rulerControl1 = new Lyquidity.UtilityLibrary.Controls.RulerControl();
            this.statusDrowing = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.pnlRulerTop = new System.Windows.Forms.Panel();
            this.pnlTop = new System.Windows.Forms.Panel();
            this.hrulerControl = new Lyquidity.UtilityLibrary.Controls.RulerControl();
            this.pnlZro = new System.Windows.Forms.Panel();
            this.cameraToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.takeImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.takeVediToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.recalculatePathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.controlToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.pnlMainDrowing.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.pnlRulerLeft.SuspendLayout();
            this.pnlLeft.SuspendLayout();
            this.statusDrowing.SuspendLayout();
            this.pnlRulerTop.SuspendLayout();
            this.pnlTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Location = new System.Drawing.Point(0, 702);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusStrip1.Size = new System.Drawing.Size(1313, 22);
            this.statusStrip1.TabIndex = 0;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel1.Location = new System.Drawing.Point(0, 30);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(281, 672);
            this.panel1.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listView2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 235);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(279, 235);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Robot\'s Area List";
            // 
            // listView2
            // 
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.HideSelection = false;
            this.listView2.Location = new System.Drawing.Point(4, 19);
            this.listView2.Margin = new System.Windows.Forms.Padding(4);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(271, 212);
            this.listView2.TabIndex = 0;
            this.listView2.UseCompatibleStateImageBehavior = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listView1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(279, 235);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Area List";
            // 
            // listView1
            // 
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(4, 19);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(271, 212);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1313, 30);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadAreaToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(60, 24);
            this.toolStripMenuItem1.Text = "Routs";
            this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
            // 
            // loadAreaToolStripMenuItem
            // 
            this.loadAreaToolStripMenuItem.Name = "loadAreaToolStripMenuItem";
            this.loadAreaToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.loadAreaToolStripMenuItem.Text = "load Area";
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(281, 30);
            this.splitter1.Margin = new System.Windows.Forms.Padding(4);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(4, 672);
            this.splitter1.TabIndex = 3;
            this.splitter1.TabStop = false;
            // 
            // pnlMainDrowing
            // 
            this.pnlMainDrowing.Controls.Add(this.pnlCanvas);
            this.pnlMainDrowing.Controls.Add(this.pnlRulerLeft);
            this.pnlMainDrowing.Controls.Add(this.statusDrowing);
            this.pnlMainDrowing.Controls.Add(this.pnlRulerTop);
            this.pnlMainDrowing.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainDrowing.Location = new System.Drawing.Point(285, 30);
            this.pnlMainDrowing.Margin = new System.Windows.Forms.Padding(4);
            this.pnlMainDrowing.Name = "pnlMainDrowing";
            this.pnlMainDrowing.Size = new System.Drawing.Size(1028, 672);
            this.pnlMainDrowing.TabIndex = 8;
            // 
            // pnlCanvas
            // 
            this.pnlCanvas.AutoScroll = true;
            this.pnlCanvas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlCanvas.ContextMenuStrip = this.contextMenuStrip1;
            this.pnlCanvas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlCanvas.Location = new System.Drawing.Point(27, 25);
            this.pnlCanvas.Margin = new System.Windows.Forms.Padding(4);
            this.pnlCanvas.Name = "pnlCanvas";
            this.pnlCanvas.Size = new System.Drawing.Size(1001, 621);
            this.pnlCanvas.TabIndex = 2;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromPointToolStripMenuItem,
            this.toPointToolStripMenuItem,
            this.cancelAndReturnHomeToolStripMenuItem,
            this.startRouteToolStripMenuItem,
            this.robotCommandToolStripMenuItem,
            this.areaToolStripMenuItem,
            this.cameraToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(244, 172);
            // 
            // fromPointToolStripMenuItem
            // 
            this.fromPointToolStripMenuItem.Name = "fromPointToolStripMenuItem";
            this.fromPointToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.fromPointToolStripMenuItem.Text = "From Point";
            // 
            // toPointToolStripMenuItem
            // 
            this.toPointToolStripMenuItem.Name = "toPointToolStripMenuItem";
            this.toPointToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.toPointToolStripMenuItem.Text = "To Point";
            // 
            // cancelAndReturnHomeToolStripMenuItem
            // 
            this.cancelAndReturnHomeToolStripMenuItem.Name = "cancelAndReturnHomeToolStripMenuItem";
            this.cancelAndReturnHomeToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.cancelAndReturnHomeToolStripMenuItem.Text = "Cancel and Return Home";
            // 
            // startRouteToolStripMenuItem
            // 
            this.startRouteToolStripMenuItem.Name = "startRouteToolStripMenuItem";
            this.startRouteToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.startRouteToolStripMenuItem.Text = "Start Route";
            // 
            // robotCommandToolStripMenuItem
            // 
            this.robotCommandToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.controlToolStripMenuItem1});
            this.robotCommandToolStripMenuItem.Name = "robotCommandToolStripMenuItem";
            this.robotCommandToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.robotCommandToolStripMenuItem.Text = "Robot";
            // 
            // areaToolStripMenuItem
            // 
            this.areaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aToolStripMenuItem1,
            this.recalculatePathToolStripMenuItem});
            this.areaToolStripMenuItem.Name = "areaToolStripMenuItem";
            this.areaToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.areaToolStripMenuItem.Text = "Route";
            // 
            // aToolStripMenuItem1
            // 
            this.aToolStripMenuItem1.Name = "aToolStripMenuItem1";
            this.aToolStripMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.aToolStripMenuItem1.Text = "Rout Data";
            // 
            // pnlRulerLeft
            // 
            this.pnlRulerLeft.Controls.Add(this.pnlLeft);
            this.pnlRulerLeft.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlRulerLeft.Location = new System.Drawing.Point(0, 25);
            this.pnlRulerLeft.Margin = new System.Windows.Forms.Padding(4);
            this.pnlRulerLeft.Name = "pnlRulerLeft";
            this.pnlRulerLeft.Size = new System.Drawing.Size(27, 621);
            this.pnlRulerLeft.TabIndex = 3;
            // 
            // pnlLeft
            // 
            this.pnlLeft.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlLeft.Controls.Add(this.rulerControl1);
            this.pnlLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlLeft.Location = new System.Drawing.Point(0, 0);
            this.pnlLeft.Margin = new System.Windows.Forms.Padding(4);
            this.pnlLeft.Name = "pnlLeft";
            this.pnlLeft.Size = new System.Drawing.Size(27, 621);
            this.pnlLeft.TabIndex = 3;
            // 
            // rulerControl1
            // 
            this.rulerControl1.ActualSize = true;
            this.rulerControl1.BackColor = System.Drawing.Color.Khaki;
            this.rulerControl1.DivisionMarkFactor = 5;
            this.rulerControl1.Divisions = 10;
            this.rulerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rulerControl1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.rulerControl1.Location = new System.Drawing.Point(0, 0);
            this.rulerControl1.MajorInterval = 1;
            this.rulerControl1.Margin = new System.Windows.Forms.Padding(4);
            this.rulerControl1.MiddleMarkFactor = 3;
            this.rulerControl1.MouseTrackingOn = true;
            this.rulerControl1.Name = "rulerControl1";
            this.rulerControl1.Orientation = Lyquidity.UtilityLibrary.Controls.enumOrientation.orVertical;
            this.rulerControl1.RulerAlignment = Lyquidity.UtilityLibrary.Controls.enumRulerAlignment.raBottomOrRight;
            this.rulerControl1.ScaleMode = Lyquidity.UtilityLibrary.Controls.enumScaleMode.smCentimetres;
            this.rulerControl1.Size = new System.Drawing.Size(25, 619);
            this.rulerControl1.StartValue = 0D;
            this.rulerControl1.TabIndex = 2;
            this.rulerControl1.VerticalNumbers = false;
            this.rulerControl1.ZoomFactor = 1D;
            // 
            // statusDrowing
            // 
            this.statusDrowing.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusDrowing.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel3,
            this.toolStripStatusLabel4});
            this.statusDrowing.Location = new System.Drawing.Point(0, 646);
            this.statusDrowing.Name = "statusDrowing";
            this.statusDrowing.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
            this.statusDrowing.Size = new System.Drawing.Size(1028, 26);
            this.statusDrowing.SizingGrip = false;
            this.statusDrowing.TabIndex = 5;
            this.statusDrowing.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(19, 20);
            this.toolStripStatusLabel1.Text = "x:";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(17, 20);
            this.toolStripStatusLabel2.Text = "0";
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            this.toolStripStatusLabel3.Size = new System.Drawing.Size(19, 20);
            this.toolStripStatusLabel3.Text = "y:";
            // 
            // toolStripStatusLabel4
            // 
            this.toolStripStatusLabel4.Name = "toolStripStatusLabel4";
            this.toolStripStatusLabel4.Size = new System.Drawing.Size(17, 20);
            this.toolStripStatusLabel4.Text = "0";
            // 
            // pnlRulerTop
            // 
            this.pnlRulerTop.Controls.Add(this.pnlTop);
            this.pnlRulerTop.Controls.Add(this.pnlZro);
            this.pnlRulerTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlRulerTop.Location = new System.Drawing.Point(0, 0);
            this.pnlRulerTop.Margin = new System.Windows.Forms.Padding(4);
            this.pnlRulerTop.Name = "pnlRulerTop";
            this.pnlRulerTop.Size = new System.Drawing.Size(1028, 25);
            this.pnlRulerTop.TabIndex = 4;
            // 
            // pnlTop
            // 
            this.pnlTop.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlTop.Controls.Add(this.hrulerControl);
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlTop.Location = new System.Drawing.Point(25, 0);
            this.pnlTop.Margin = new System.Windows.Forms.Padding(4);
            this.pnlTop.Name = "pnlTop";
            this.pnlTop.Size = new System.Drawing.Size(1003, 25);
            this.pnlTop.TabIndex = 2;
            // 
            // hrulerControl
            // 
            this.hrulerControl.ActualSize = true;
            this.hrulerControl.BackColor = System.Drawing.Color.Khaki;
            this.hrulerControl.DivisionMarkFactor = 5;
            this.hrulerControl.Divisions = 10;
            this.hrulerControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hrulerControl.ForeColor = System.Drawing.SystemColors.ControlText;
            this.hrulerControl.Location = new System.Drawing.Point(0, 0);
            this.hrulerControl.MajorInterval = 1;
            this.hrulerControl.Margin = new System.Windows.Forms.Padding(4);
            this.hrulerControl.MiddleMarkFactor = 3;
            this.hrulerControl.MouseTrackingOn = true;
            this.hrulerControl.Name = "hrulerControl";
            this.hrulerControl.Orientation = Lyquidity.UtilityLibrary.Controls.enumOrientation.orHorizontal;
            this.hrulerControl.RulerAlignment = Lyquidity.UtilityLibrary.Controls.enumRulerAlignment.raBottomOrRight;
            this.hrulerControl.ScaleMode = Lyquidity.UtilityLibrary.Controls.enumScaleMode.smCentimetres;
            this.hrulerControl.Size = new System.Drawing.Size(1001, 23);
            this.hrulerControl.StartValue = 0D;
            this.hrulerControl.TabIndex = 1;
            this.hrulerControl.VerticalNumbers = false;
            this.hrulerControl.ZoomFactor = 1D;
            // 
            // pnlZro
            // 
            this.pnlZro.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnlZro.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlZro.Location = new System.Drawing.Point(0, 0);
            this.pnlZro.Margin = new System.Windows.Forms.Padding(4);
            this.pnlZro.Name = "pnlZro";
            this.pnlZro.Size = new System.Drawing.Size(25, 25);
            this.pnlZro.TabIndex = 1;
            // 
            // cameraToolStripMenuItem
            // 
            this.cameraToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.takeImageToolStripMenuItem,
            this.takeVediToolStripMenuItem,
            this.controlToolStripMenuItem});
            this.cameraToolStripMenuItem.Name = "cameraToolStripMenuItem";
            this.cameraToolStripMenuItem.Size = new System.Drawing.Size(243, 24);
            this.cameraToolStripMenuItem.Text = "Camera";
            // 
            // takeImageToolStripMenuItem
            // 
            this.takeImageToolStripMenuItem.Name = "takeImageToolStripMenuItem";
            this.takeImageToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.takeImageToolStripMenuItem.Text = "Image";
            // 
            // takeVediToolStripMenuItem
            // 
            this.takeVediToolStripMenuItem.Name = "takeVediToolStripMenuItem";
            this.takeVediToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.takeVediToolStripMenuItem.Text = "Video";
            // 
            // controlToolStripMenuItem
            // 
            this.controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            this.controlToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.controlToolStripMenuItem.Text = "Control";
            // 
            // recalculatePathToolStripMenuItem
            // 
            this.recalculatePathToolStripMenuItem.Name = "recalculatePathToolStripMenuItem";
            this.recalculatePathToolStripMenuItem.Size = new System.Drawing.Size(224, 26);
            this.recalculatePathToolStripMenuItem.Text = "Recalculate Path";
            // 
            // controlToolStripMenuItem1
            // 
            this.controlToolStripMenuItem1.Name = "controlToolStripMenuItem1";
            this.controlToolStripMenuItem1.Size = new System.Drawing.Size(224, 26);
            this.controlToolStripMenuItem1.Text = "Control";
            // 
            // frmRouteCommand
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1313, 724);
            this.Controls.Add(this.pnlMainDrowing);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "frmRouteCommand";
            this.Text = "frmRouteCommand";
            this.panel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.pnlMainDrowing.ResumeLayout(false);
            this.pnlMainDrowing.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.pnlRulerLeft.ResumeLayout(false);
            this.pnlLeft.ResumeLayout(false);
            this.statusDrowing.ResumeLayout(false);
            this.statusDrowing.PerformLayout();
            this.pnlRulerTop.ResumeLayout(false);
            this.pnlTop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Panel pnlMainDrowing;
        private System.Windows.Forms.Panel pnlCanvas;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fromPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toPointToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelAndReturnHomeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem startRouteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem robotCommandToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem areaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aToolStripMenuItem1;
        private System.Windows.Forms.Panel pnlRulerLeft;
        private System.Windows.Forms.Panel pnlLeft;
        private Lyquidity.UtilityLibrary.Controls.RulerControl rulerControl1;
        private System.Windows.Forms.StatusStrip statusDrowing;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel4;
        private System.Windows.Forms.Panel pnlRulerTop;
        private System.Windows.Forms.Panel pnlTop;
        private Lyquidity.UtilityLibrary.Controls.RulerControl hrulerControl;
        private System.Windows.Forms.Panel pnlZro;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadAreaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cameraToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem takeImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem takeVediToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem controlToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem recalculatePathToolStripMenuItem;
    }
}