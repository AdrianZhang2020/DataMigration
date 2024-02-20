namespace DataMigration;

public partial class ScrollableMessageBox : Form
{
    private Sunny.UI.UITextBox textBox;
    public ScrollableMessageBox(string title, string message)
    {
        this.Text = title; // 设置对话框的标题  
        this.Size = new System.Drawing.Size(500, 300); // 设置对话框的大小  
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MaximizeBox = false;

        // 创建文本框并设置滚动条  
        textBox = new Sunny.UI.UITextBox
        {
            Multiline = true,
            Font = Font = new Font("宋体", 10F, FontStyle.Regular, GraphicsUnit.Point, 134),
        ShowScrollBar = true,
            ReadOnly = true,
            Text = message,
            Size = new System.Drawing.Size(this.Width - 10, this.Height - 35)
        };

        // 将控件添加到对话框中
        this.Controls.Add(textBox);
    }
}
