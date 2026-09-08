# Laptop Keyboard Disabler (Tắt/Bật Bàn Phím Laptop Tức Thì)

Ứng dụng Windows nhỏ gọn, tiện ích cho phép vô hiệu hóa bàn phím tích hợp trên laptop khi bạn sử dụng bàn phím rời USB/Bluetooth mà **hoàn toàn không cần khởi động lại máy (no restart)**, sử dụng bộ lọc cấp độ nhân hệ thống **Interception Driver**.

---

## 🌟 Các Tính Năng Nổi Bật

1. **Bật / Tắt tức thì trong 0 giây**:
   - Sử dụng kernel driver filter Interception, chặn trực tiếp tín hiệu từ bàn phím laptop mà không gây giật lag hay ảnh hưởng đến bàn phím USB.
   - Bàn phím rời (USB/Bluetooth), chuột và touchpad vẫn hoạt động 100% mượt mà.
2. **Khởi động cùng Windows (Tùy chọn Bật/Tắt)**:
   - Tích hợp qua Windows Scheduled Task chạy mức `HighestAvailable`.
   - Tự động chạy ngầm dưới khay hệ thống khi bạn bật máy mà **không bao giờ bị Windows UAC hỏi xác nhận phiền phức**.
3. **Chạy ngầm ở Khay Hệ Thống (System Tray)**:
   - Icon động đổi màu:
     - 🟢 **Xanh lá**: Bàn phím laptop đang BẬT.
     - 🔴 **Đỏ**: Bàn phím laptop đã TẮT.
   - Menu chuột phải tại khay để bật/tắt nhanh 1-click.
   - Khi bấm nút `X` đóng cửa sổ, ứng dụng tự động thu nhỏ xuống khay.
4. **Phím tắt toàn cục (Global Hotkey)**:
   - Bấm `Ctrl + Alt + K` ở bất cứ đâu để đảo trạng thái Bật/Tắt bàn phím laptop tức thì mà không cần mở cửa sổ.
5. **Nhận diện bàn phím thông minh (1-Click Identify)**:
   - Tự động phát hiện thiết bị laptop (`ACPI\KBC8042...`).
   - Có nút **"🎯 Nhận Diện Bằng 1 Phím"**: Nhấn nút này và gõ 1 phím bất kỳ trên bàn phím laptop để ứng dụng tự động ghi nhớ và khóa chính xác thiết bị.
6. **Bảo vệ an toàn khi thoát (Safe Exit)**:
   - Nếu bạn chọn Thoát (Exit) khi phím laptop đang tắt, ứng dụng sẽ hỏi bạn có muốn bật lại phím laptop trước khi tắt hẳn không.

---

## 🚀 Hướng Dẫn Sử Dụng

### Cài đặt lần đầu:
1. Mở ứng dụng `LaptopKeyboardDisabler.exe`.
2. Nếu máy chưa có driver Interception, ứng dụng sẽ hiện thông báo màu vàng cam. Bấm **📥 CÀI ĐẶT DRIVER INTERCEPTION NGAY**.
3. Bấm **Yes** khi hộp thoại UAC hỏi. Sau khi cài xong, chọn **Khởi động lại máy (Restart)** 1 lần duy nhất để Windows nạp driver.

### Sử dụng hàng ngày (Không cần restart nữa):
- Bấm **🔴 TẮT BÀN PHÍM LAPTOP** (hoặc tổ hợp phím **`Ctrl + Alt + K`**).
- Tích chọn `🚀 Khởi động cùng Windows` và `🔒 Luôn tự động tắt phím laptop mỗi khi mở máy` để ứng dụng tự động xử lý mỗi khi bạn bật máy tính.

---

## 🛠 Hướng Dẫn Build Dự Án

Yêu cầu: .NET 8 / .NET 10 SDK

```bash
# Clone repository
git clone https://github.com/dnghngqun/disable-keyboard.git
cd disable-keyboard

# Build
dotnet build -c Release

# Xuất file chạy độc lập
dotnet publish -c Release -r win-x64 --self-contained false -p:PublishSingleFile=true -o ./publish
```

---

## 📜 Giấy Phép & Công Nghệ

- C# / Windows Forms (.NET 10) Native Windows x64
- [Interception Driver](https://github.com/oblitum/Interception) / [InputInterceptor](https://github.com/0x2E757/InputInterceptor)
