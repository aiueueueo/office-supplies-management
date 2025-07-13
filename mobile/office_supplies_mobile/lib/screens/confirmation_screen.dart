import 'package:flutter/material.dart';
import '../models/department.dart';
import '../models/item.dart';
import '../models/transaction_request.dart';
import '../services/api_service.dart';
import 'result_screen.dart';

class ConfirmationScreen extends StatefulWidget {
  final Department department;
  final Item item;
  final TransactionRequest request;

  const ConfirmationScreen({
    super.key,
    required this.department,
    required this.item,
    required this.request,
  });

  @override
  State<ConfirmationScreen> createState() => _ConfirmationScreenState();
}

class _ConfirmationScreenState extends State<ConfirmationScreen> {
  final ApiService _apiService = ApiService();
  bool _isProcessing = false;

  Future<void> _processStockOut() async {
    setState(() {
      _isProcessing = true;
    });

    try {
      final success = await _apiService.processStockOut(widget.request);
      
      if (!mounted) return;

      Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (context) => ResultScreen(
            success: success,
            department: widget.department,
            item: widget.item,
            quantity: widget.request.quantity,
          ),
        ),
      );
    } catch (e) {
      if (!mounted) return;
      
      Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (context) => ResultScreen(
            success: false,
            department: widget.department,
            item: widget.item,
            quantity: widget.request.quantity,
            errorMessage: e.toString(),
          ),
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    final afterStock = widget.item.currentStock - widget.request.quantity;

    return Scaffold(
      appBar: AppBar(
        title: const Text('出庫確認'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        automaticallyImplyLeading: !_isProcessing,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              '以下の内容で出庫処理を実行します',
              style: TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 24),

            // 確認情報カード
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  children: [
                    _buildInfoRow('部署', widget.department.departmentName),
                    const Divider(),
                    _buildInfoRow('物品名', widget.item.itemName),
                    const Divider(),
                    _buildInfoRow('物品コード', widget.item.itemCode),
                    const Divider(),
                    _buildInfoRow(
                      '出庫数量',
                      '${widget.request.quantity} ${widget.item.unit}',
                      valueStyle: const TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: Colors.red,
                      ),
                    ),
                    const Divider(),
                    _buildInfoRow(
                      '処理前在庫',
                      '${widget.item.currentStock} ${widget.item.unit}',
                    ),
                    const Divider(),
                    _buildInfoRow(
                      '処理後在庫',
                      '$afterStock ${widget.item.unit}',
                      valueStyle: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                        color: afterStock <= widget.item.minimumStock 
                            ? Colors.red 
                            : Colors.green,
                      ),
                    ),
                    if (widget.request.remarks != null && widget.request.remarks!.isNotEmpty) ...[
                      const Divider(),
                      _buildInfoRow('備考', widget.request.remarks!),
                    ],
                  ],
                ),
              ),
            ),

            if (afterStock <= widget.item.minimumStock) ...[
              const SizedBox(height: 16),
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.red.shade50,
                  border: Border.all(color: Colors.red),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.warning, color: Colors.red),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        '処理後の在庫が最小在庫数（${widget.item.minimumStock} ${widget.item.unit}）を下回ります',
                        style: const TextStyle(color: Colors.red),
                      ),
                    ),
                  ],
                ),
              ),
            ],

            const Spacer(),

            if (_isProcessing) ...[
              const Center(
                child: Column(
                  children: [
                    CircularProgressIndicator(),
                    SizedBox(height: 16),
                    Text('処理中...'),
                  ],
                ),
              ),
            ] else ...[
              // ボタン
              Row(
                children: [
                  Expanded(
                    child: OutlinedButton(
                      onPressed: () => Navigator.pop(context),
                      style: OutlinedButton.styleFrom(
                        padding: const EdgeInsets.symmetric(vertical: 16),
                      ),
                      child: const Text('戻る'),
                    ),
                  ),
                  const SizedBox(width: 16),
                  Expanded(
                    child: ElevatedButton(
                      onPressed: _processStockOut,
                      style: ElevatedButton.styleFrom(
                        backgroundColor: Colors.red,
                        foregroundColor: Colors.white,
                        padding: const EdgeInsets.symmetric(vertical: 16),
                      ),
                      child: const Text(
                        '出庫実行',
                        style: TextStyle(fontSize: 16),
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ],
        ),
      ),
    );
  }

  Widget _buildInfoRow(String label, String value, {TextStyle? valueStyle}) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 8.0),
      child: Row(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          SizedBox(
            width: 100,
            child: Text(
              label,
              style: const TextStyle(
                fontWeight: FontWeight.w500,
                color: Colors.grey,
              ),
            ),
          ),
          Expanded(
            child: Text(
              value,
              style: valueStyle ?? const TextStyle(fontSize: 16),
            ),
          ),
        ],
      ),
    );
  }
}