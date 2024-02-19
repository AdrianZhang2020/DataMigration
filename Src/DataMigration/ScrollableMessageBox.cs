namespace DataMigration;

public partial class ScrollableMessageBox : Form
{
    private TextBox textBox;
    public ScrollableMessageBox(string title, string message)
    {
        this.Text = title; // 设置对话框的标题  
        this.Size = new System.Drawing.Size(500, 300); // 设置对话框的大小  
        this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        this.StartPosition = FormStartPosition.CenterScreen;

        // 创建文本框并设置滚动条  
        textBox = new TextBox
        {
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            ReadOnly = true,
            Text = message,
            Size = new System.Drawing.Size(this.Width - 10, this.Height - 30)
        };

        // 将控件添加到对话框中
        this.Controls.Add(textBox);
    }
}
