import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import '../models/department.dart';
import '../models/item.dart';
import '../services/api_service.dart';
import 'quantity_input_screen.dart';

class BarcodeScannerScreen extends StatefulWidget {
  final Department department;

  const BarcodeScannerScreen({
    super.key,
    required this.department,
  });

  @override
  State<BarcodeScannerScreen> createState() => _BarcodeScannerScreenState();
}

class _BarcodeScannerScreenState extends State<BarcodeScannerScreen> {
  final ApiService _apiService = ApiService();
  final TextEditingController _barcodeController = TextEditingController();
  bool _isScanning = false;

  // カメラスキャンのシミュレーション（実際の実装ではqr_code_scannerを使用）
  void _simulateBarcodeScan() {
    // テスト用のダミーバーコード
    const testBarcodes = [
      '4901681181814', // ボールペン（黒）
      '4901681181821', // ボールペン（赤）
      '4902505544194', // A4コピー用紙
    ];

    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('テスト用バーコード'),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          children: testBarcodes.map((barcode) {
            return ListTile(
              title: Text(barcode),
              onTap: () {
                Navigator.pop(context);
                _processBarcode(barcode);
              },
            );
          }).toList(),
        ),
      ),
    );
  }

  Future<void> _processBarcode(String barcode) async {
    setState(() {
      _isScanning = true;
    });

    try {
      final item = await _apiService.getItemByBarcode(barcode);
      
      if (!mounted) return;

      if (item != null) {
        Navigator.push(
          context,
          MaterialPageRoute(
            builder: (context) => QuantityInputScreen(
              department: widget.department,
              item: item,
            ),
          ),
        );
      } else {
        _showErrorDialog('物品が見つかりません', 'バーコード「$barcode」に対応する物品が登録されていません。');
      }
    } catch (e) {
      if (!mounted) return;
      _showErrorDialog('エラー', 'データの取得に失敗しました。\n$e');
    } finally {
      setState(() {
        _isScanning = false;
      });
    }
  }

  void _showErrorDialog(String title, String message) {
    showDialog(
      context: context,
      builder: (context) => AlertDialog(
        title: Text(title),
        content: Text(message),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context),
            child: const Text('OK'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('バーコードスキャン'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Card(
              color: Colors.blue.shade50,
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      '選択した部署',
                      style: TextStyle(
                        fontSize: 14,
                        color: Colors.grey,
                      ),
                    ),
                    const SizedBox(height: 4),
                    Text(
                      widget.department.departmentName,
                      style: const TextStyle(
                        fontSize: 18,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                  ],
                ),
              ),
            ),
            const SizedBox(height: 24),
            const Text(
              'バーコード/QRコードをスキャン',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 16),
            Container(
              width: double.infinity,
              height: 200,
              decoration: BoxDecoration(
                border: Border.all(color: Colors.grey),
                borderRadius: BorderRadius.circular(8),
              ),
              child: _isScanning
                  ? const Center(
                      child: CircularProgressIndicator(),
                    )
                  : Column(
                      mainAxisAlignment: MainAxisAlignment.center,
                      children: [
                        const Icon(
                          Icons.qr_code_scanner,
                          size: 64,
                          color: Colors.grey,
                        ),
                        const SizedBox(height: 16),
                        ElevatedButton(
                          onPressed: _simulateBarcodeScan,
                          child: const Text('カメラでスキャン'),
                        ),
                      ],
                    ),
            ),
            const SizedBox(height: 24),
            const Text(
              'または手動入力',
              style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.w500,
              ),
            ),
            const SizedBox(height: 8),
            TextField(
              controller: _barcodeController,
              decoration: const InputDecoration(
                labelText: 'バーコード/QRコード',
                border: OutlineInputBorder(),
                hintText: 'バーコードを入力してください',
              ),
              onSubmitted: (value) {
                if (value.isNotEmpty) {
                  _processBarcode(value);
                }
              },
            ),
            const SizedBox(height: 16),
            SizedBox(
              width: double.infinity,
              height: 50,
              child: ElevatedButton(
                onPressed: _barcodeController.text.isEmpty || _isScanning
                    ? null
                    : () => _processBarcode(_barcodeController.text),
                child: _isScanning
                    ? const CircularProgressIndicator(
                        valueColor: AlwaysStoppedAnimation<Color>(Colors.white),
                      )
                    : const Text('検索'),
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  void dispose() {
    _barcodeController.dispose();
    super.dispose();
  }
}