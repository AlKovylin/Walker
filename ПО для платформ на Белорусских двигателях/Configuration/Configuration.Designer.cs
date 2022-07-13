namespace Custom_setting
{
    partial class Configuration
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Configuration));
            this.comboBox_Port = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBox_Sensor = new System.Windows.Forms.ComboBox();
            this.textBox_Device = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.button_Add = new System.Windows.Forms.Button();
            this.button_Save = new System.Windows.Forms.Button();
            this.button_Clear = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button_ClosePort = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.comboBox_ViewPort = new System.Windows.Forms.ComboBox();
            this.button_ReadPort = new System.Windows.Forms.Button();
            this.textBox_DataPort = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label_Speed3 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.trackBar_Speed3 = new System.Windows.Forms.TrackBar();
            this.buttonDn3 = new System.Windows.Forms.Button();
            this.buttonUp3 = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.label_Speed2 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.trackBar_Speed2 = new System.Windows.Forms.TrackBar();
            this.buttonDn2 = new System.Windows.Forms.Button();
            this.buttonUp2 = new System.Windows.Forms.Button();
            this.label13 = new System.Windows.Forms.Label();
            this.label_Speed1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.trackBar_Speed1 = new System.Windows.Forms.TrackBar();
            this.buttonDn1 = new System.Windows.Forms.Button();
            this.buttonUp1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.buttonKalibr = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Speed3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Speed2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Speed1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox_Port
            // 
            this.comboBox_Port.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Port.FormattingEnabled = true;
            this.comboBox_Port.Location = new System.Drawing.Point(10, 44);
            this.comboBox_Port.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox_Port.Name = "comboBox_Port";
            this.comboBox_Port.Size = new System.Drawing.Size(96, 21);
            this.comboBox_Port.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 28);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Порт";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(128, 28);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Датчик";
            // 
            // comboBox_Sensor
            // 
            this.comboBox_Sensor.BackColor = System.Drawing.SystemColors.HighlightText;
            this.comboBox_Sensor.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_Sensor.FormattingEnabled = true;
            this.comboBox_Sensor.Location = new System.Drawing.Point(128, 44);
            this.comboBox_Sensor.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox_Sensor.Name = "comboBox_Sensor";
            this.comboBox_Sensor.Size = new System.Drawing.Size(140, 21);
            this.comboBox_Sensor.TabIndex = 2;
            // 
            // textBox_Device
            // 
            this.textBox_Device.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox_Device.Location = new System.Drawing.Point(364, 44);
            this.textBox_Device.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_Device.Multiline = true;
            this.textBox_Device.Name = "textBox_Device";
            this.textBox_Device.ReadOnly = true;
            this.textBox_Device.Size = new System.Drawing.Size(175, 91);
            this.textBox_Device.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(362, 28);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Текущее сопоставление";
            // 
            // button_Add
            // 
            this.button_Add.Location = new System.Drawing.Point(289, 43);
            this.button_Add.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_Add.Name = "button_Add";
            this.button_Add.Size = new System.Drawing.Size(56, 24);
            this.button_Add.TabIndex = 6;
            this.button_Add.Text = ">>";
            this.button_Add.UseVisualStyleBackColor = true;
            this.button_Add.Click += new System.EventHandler(this.button_Add_Click);
            // 
            // button_Save
            // 
            this.button_Save.Enabled = false;
            this.button_Save.Location = new System.Drawing.Point(547, 76);
            this.button_Save.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_Save.Name = "button_Save";
            this.button_Save.Size = new System.Drawing.Size(68, 28);
            this.button_Save.TabIndex = 7;
            this.button_Save.Text = "Сохранить";
            this.button_Save.UseVisualStyleBackColor = true;
            this.button_Save.Click += new System.EventHandler(this.buttonSave_Click);
            // 
            // button_Clear
            // 
            this.button_Clear.Location = new System.Drawing.Point(547, 44);
            this.button_Clear.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_Clear.Name = "button_Clear";
            this.button_Clear.Size = new System.Drawing.Size(68, 28);
            this.button_Clear.TabIndex = 8;
            this.button_Clear.Text = "Очистить";
            this.button_Clear.UseVisualStyleBackColor = true;
            this.button_Clear.Click += new System.EventHandler(this.buttonClear_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button_ClosePort);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.comboBox_ViewPort);
            this.groupBox1.Controls.Add(this.button_ReadPort);
            this.groupBox1.Controls.Add(this.textBox_DataPort);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Location = new System.Drawing.Point(9, 169);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox1.Size = new System.Drawing.Size(210, 116);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Чтение датчиков";
            // 
            // button_ClosePort
            // 
            this.button_ClosePort.Enabled = false;
            this.button_ClosePort.Location = new System.Drawing.Point(112, 73);
            this.button_ClosePort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_ClosePort.Name = "button_ClosePort";
            this.button_ClosePort.Size = new System.Drawing.Size(86, 24);
            this.button_ClosePort.TabIndex = 5;
            this.button_ClosePort.Text = "Закрыть порт";
            this.button_ClosePort.UseVisualStyleBackColor = true;
            this.button_ClosePort.Click += new System.EventHandler(this.buttonClosePort_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(110, 32);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Данные";
            // 
            // comboBox_ViewPort
            // 
            this.comboBox_ViewPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_ViewPort.FormattingEnabled = true;
            this.comboBox_ViewPort.Location = new System.Drawing.Point(10, 49);
            this.comboBox_ViewPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.comboBox_ViewPort.Name = "comboBox_ViewPort";
            this.comboBox_ViewPort.Size = new System.Drawing.Size(86, 21);
            this.comboBox_ViewPort.TabIndex = 3;
            // 
            // button_ReadPort
            // 
            this.button_ReadPort.Location = new System.Drawing.Point(10, 73);
            this.button_ReadPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.button_ReadPort.Name = "button_ReadPort";
            this.button_ReadPort.Size = new System.Drawing.Size(86, 24);
            this.button_ReadPort.TabIndex = 2;
            this.button_ReadPort.Text = "Читать порт";
            this.button_ReadPort.UseVisualStyleBackColor = true;
            this.button_ReadPort.Click += new System.EventHandler(this.buttonReadPort_Click);
            // 
            // textBox_DataPort
            // 
            this.textBox_DataPort.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.textBox_DataPort.Location = new System.Drawing.Point(112, 49);
            this.textBox_DataPort.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.textBox_DataPort.Name = "textBox_DataPort";
            this.textBox_DataPort.ReadOnly = true;
            this.textBox_DataPort.Size = new System.Drawing.Size(86, 20);
            this.textBox_DataPort.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 32);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Выбор порта";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label_Speed3);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.trackBar_Speed3);
            this.groupBox2.Controls.Add(this.buttonDn3);
            this.groupBox2.Controls.Add(this.buttonUp3);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label_Speed2);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.trackBar_Speed2);
            this.groupBox2.Controls.Add(this.buttonDn2);
            this.groupBox2.Controls.Add(this.buttonUp2);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.label_Speed1);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.trackBar_Speed1);
            this.groupBox2.Controls.Add(this.buttonDn1);
            this.groupBox2.Controls.Add(this.buttonUp1);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Location = new System.Drawing.Point(231, 170);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox2.Size = new System.Drawing.Size(400, 185);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Управление двигателями";
            // 
            // label_Speed3
            // 
            this.label_Speed3.Location = new System.Drawing.Point(333, 62);
            this.label_Speed3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Speed3.Name = "label_Speed3";
            this.label_Speed3.Size = new System.Drawing.Size(26, 14);
            this.label_Speed3.TabIndex = 21;
            this.label_Speed3.Text = "0";
            this.label_Speed3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(357, 63);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(22, 13);
            this.label15.TabIndex = 20;
            this.label15.Text = "ед.";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(280, 62);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(58, 13);
            this.label16.TabIndex = 19;
            this.label16.Text = "Скорость:";
            // 
            // trackBar_Speed3
            // 
            this.trackBar_Speed3.Location = new System.Drawing.Point(272, 92);
            this.trackBar_Speed3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.trackBar_Speed3.Maximum = 255;
            this.trackBar_Speed3.Name = "trackBar_Speed3";
            this.trackBar_Speed3.Size = new System.Drawing.Size(120, 45);
            this.trackBar_Speed3.TabIndex = 18;
            this.trackBar_Speed3.Scroll += new System.EventHandler(this.trackBar_Speed3_Scroll);
            // 
            // buttonDn3
            // 
            this.buttonDn3.Location = new System.Drawing.Point(335, 137);
            this.buttonDn3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonDn3.Name = "buttonDn3";
            this.buttonDn3.Size = new System.Drawing.Size(56, 27);
            this.buttonDn3.TabIndex = 17;
            this.buttonDn3.Text = "Вниз";
            this.buttonDn3.UseVisualStyleBackColor = true;
            this.buttonDn3.Click += new System.EventHandler(this.buttonDn3_Click);
            // 
            // buttonUp3
            // 
            this.buttonUp3.Location = new System.Drawing.Point(274, 137);
            this.buttonUp3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonUp3.Name = "buttonUp3";
            this.buttonUp3.Size = new System.Drawing.Size(56, 27);
            this.buttonUp3.TabIndex = 16;
            this.buttonUp3.Text = "Вверх";
            this.buttonUp3.UseVisualStyleBackColor = true;
            this.buttonUp3.Click += new System.EventHandler(this.buttonUp3_Click);
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(12, 31);
            this.label17.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(111, 13);
            this.label17.TabIndex = 15;
            this.label17.Text = "Задний двигатель(1)";
            // 
            // label_Speed2
            // 
            this.label_Speed2.Location = new System.Drawing.Point(200, 62);
            this.label_Speed2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Speed2.Name = "label_Speed2";
            this.label_Speed2.Size = new System.Drawing.Size(26, 14);
            this.label_Speed2.TabIndex = 14;
            this.label_Speed2.Text = "0";
            this.label_Speed2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(224, 63);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(22, 13);
            this.label11.TabIndex = 13;
            this.label11.Text = "ед.";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(146, 62);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(58, 13);
            this.label12.TabIndex = 12;
            this.label12.Text = "Скорость:";
            // 
            // trackBar_Speed2
            // 
            this.trackBar_Speed2.Location = new System.Drawing.Point(138, 92);
            this.trackBar_Speed2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.trackBar_Speed2.Maximum = 255;
            this.trackBar_Speed2.Name = "trackBar_Speed2";
            this.trackBar_Speed2.Size = new System.Drawing.Size(120, 45);
            this.trackBar_Speed2.TabIndex = 11;
            this.trackBar_Speed2.Scroll += new System.EventHandler(this.trackBar_Speed2_Scroll);
            // 
            // buttonDn2
            // 
            this.buttonDn2.Location = new System.Drawing.Point(202, 137);
            this.buttonDn2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonDn2.Name = "buttonDn2";
            this.buttonDn2.Size = new System.Drawing.Size(56, 27);
            this.buttonDn2.TabIndex = 10;
            this.buttonDn2.Text = "Вниз";
            this.buttonDn2.UseVisualStyleBackColor = true;
            this.buttonDn2.Click += new System.EventHandler(this.buttonDn2_Click);
            // 
            // buttonUp2
            // 
            this.buttonUp2.Location = new System.Drawing.Point(141, 137);
            this.buttonUp2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonUp2.Name = "buttonUp2";
            this.buttonUp2.Size = new System.Drawing.Size(56, 27);
            this.buttonUp2.TabIndex = 9;
            this.buttonUp2.Text = "Вверх";
            this.buttonUp2.UseVisualStyleBackColor = true;
            this.buttonUp2.Click += new System.EventHandler(this.buttonUp2_Click);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(280, 32);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(114, 13);
            this.label13.TabIndex = 8;
            this.label13.Text = "Правый двигатель(3)";
            // 
            // label_Speed1
            // 
            this.label_Speed1.Location = new System.Drawing.Point(65, 62);
            this.label_Speed1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_Speed1.Name = "label_Speed1";
            this.label_Speed1.Size = new System.Drawing.Size(26, 14);
            this.label_Speed1.TabIndex = 7;
            this.label_Speed1.Text = "0";
            this.label_Speed1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(90, 63);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(22, 13);
            this.label8.TabIndex = 6;
            this.label8.Text = "ед.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(12, 62);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Скорость:";
            // 
            // trackBar_Speed1
            // 
            this.trackBar_Speed1.Location = new System.Drawing.Point(4, 92);
            this.trackBar_Speed1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.trackBar_Speed1.Maximum = 255;
            this.trackBar_Speed1.Name = "trackBar_Speed1";
            this.trackBar_Speed1.Size = new System.Drawing.Size(120, 45);
            this.trackBar_Speed1.TabIndex = 3;
            this.trackBar_Speed1.Scroll += new System.EventHandler(this.trackBar_Speed1_Scroll);
            // 
            // buttonDn1
            // 
            this.buttonDn1.BackColor = System.Drawing.SystemColors.Control;
            this.buttonDn1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonDn1.Location = new System.Drawing.Point(68, 137);
            this.buttonDn1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonDn1.Name = "buttonDn1";
            this.buttonDn1.Size = new System.Drawing.Size(56, 27);
            this.buttonDn1.TabIndex = 2;
            this.buttonDn1.Text = "Вниз";
            this.buttonDn1.UseVisualStyleBackColor = false;
            this.buttonDn1.Click += new System.EventHandler(this.buttonDn1_Click);
            // 
            // buttonUp1
            // 
            this.buttonUp1.BackColor = System.Drawing.SystemColors.Control;
            this.buttonUp1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.buttonUp1.Location = new System.Drawing.Point(7, 137);
            this.buttonUp1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonUp1.Name = "buttonUp1";
            this.buttonUp1.Size = new System.Drawing.Size(56, 27);
            this.buttonUp1.TabIndex = 1;
            this.buttonUp1.Text = "Вверх";
            this.buttonUp1.UseVisualStyleBackColor = false;
            this.buttonUp1.Click += new System.EventHandler(this.buttonUp1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(146, 31);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(108, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Левый двигатель(2)";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label20);
            this.groupBox3.Controls.Add(this.label19);
            this.groupBox3.Controls.Add(this.textBox_Device);
            this.groupBox3.Controls.Add(this.comboBox_Port);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.button_Clear);
            this.groupBox3.Controls.Add(this.comboBox_Sensor);
            this.groupBox3.Controls.Add(this.button_Save);
            this.groupBox3.Controls.Add(this.label2);
            this.groupBox3.Controls.Add(this.button_Add);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Location = new System.Drawing.Point(9, 10);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox3.Size = new System.Drawing.Size(622, 154);
            this.groupBox3.TabIndex = 12;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Сопоставление датчиков и устройств";
            // 
            // label20
            // 
            this.label20.AutoEllipsis = true;
            this.label20.Location = new System.Drawing.Point(10, 75);
            this.label20.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(349, 75);
            this.label20.TabIndex = 6;
            this.label20.Text = resources.GetString("label20.Text");
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(288, 28);
            this.label19.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(57, 13);
            this.label19.TabIndex = 9;
            this.label19.Text = "Добавить";
            // 
            // buttonKalibr
            // 
            this.buttonKalibr.Location = new System.Drawing.Point(112, 17);
            this.buttonKalibr.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.buttonKalibr.Name = "buttonKalibr";
            this.buttonKalibr.Size = new System.Drawing.Size(85, 36);
            this.buttonKalibr.TabIndex = 22;
            this.buttonKalibr.Text = "Выполнить калибровку";
            this.buttonKalibr.UseVisualStyleBackColor = true;
            this.buttonKalibr.Click += new System.EventHandler(this.buttonKalibr_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.buttonKalibr);
            this.groupBox4.Location = new System.Drawing.Point(9, 290);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.groupBox4.Size = new System.Drawing.Size(210, 65);
            this.groupBox4.TabIndex = 23;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Калибровка";
            // 
            // Configuration
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(641, 368);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.MaximumSize = new System.Drawing.Size(657, 407);
            this.MinimumSize = new System.Drawing.Size(657, 407);
            this.Name = "Configuration";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Настройка";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Speed3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Speed2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_Speed1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox_Port;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox_Sensor;
        private System.Windows.Forms.TextBox textBox_Device;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button_Add;
        private System.Windows.Forms.Button button_Save;
        private System.Windows.Forms.Button button_Clear;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button_ClosePort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox comboBox_ViewPort;
        private System.Windows.Forms.Button button_ReadPort;
        private System.Windows.Forms.TextBox textBox_DataPort;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TrackBar trackBar_Speed1;
        private System.Windows.Forms.Button buttonDn1;
        private System.Windows.Forms.Button buttonUp1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label_Speed1;
        private System.Windows.Forms.Label label_Speed3;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TrackBar trackBar_Speed3;
        private System.Windows.Forms.Button buttonDn3;
        private System.Windows.Forms.Button buttonUp3;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label_Speed2;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TrackBar trackBar_Speed2;
        private System.Windows.Forms.Button buttonDn2;
        private System.Windows.Forms.Button buttonUp2;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button buttonKalibr;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.GroupBox groupBox4;
    }
}

