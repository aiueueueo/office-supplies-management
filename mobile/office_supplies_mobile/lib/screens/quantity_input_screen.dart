import 'package:flutter/material.dart';
import '../models/department.dart';
import '../models/item.dart';
import '../models/transaction_request.dart';
import '../services/api_service.dart';
import 'confirmation_screen.dart';

class QuantityInputScreen extends StatefulWidget {
  final Department department;
  final Item item;

  const QuantityInputScreen({
    super.key,
    required this.department,
    required this.item,
  });

  @override
  State<QuantityInputScreen> createState() => _QuantityInputScreenState();
}

class _QuantityInputScreenState extends State<QuantityInputScreen> {
  final TextEditingController _quantityController = TextEditingController(text: '1');
  final TextEditingController _remarksController = TextEditingController();
  int _quantity = 1;

  void _incrementQuantity() {
    setState(() {
      _quantity++;
      _quantityController.text = _quantity.toString();
    });
  }

  void _decrementQuantity() {
    if (_quantity > 1) {
      setState(() {
        _quantity--;
        _quantityController.text = _quantity.toString();
      });
    }
  }

  void _onQuantityChanged(String value) {
    final newQuantity = int.tryParse(value);
    if (newQuantity != null && newQuantity > 0) {
      setState(() {
        _quantity = newQuantity;
      });
    }
  }

  bool get _canProceed {
    return _quantity > 0 && _quantity <= widget.item.currentStock;
  }

  void _proceed() {
    final request = TransactionRequest(
      itemId: widget.item.itemId,
      departmentId: widget.department.departmentId,
      quantity: _quantity,
      remarks: _remarksController.text.isEmpty ? null : _remarksController.text,
      processedBy: widget.department.departmentName, // 簡易実装
    );

    Navigator.push(
      context,
      MaterialPageRoute(
        builder: (context) => ConfirmationScreen(
          department: widget.department,
          item: widget.item,
          request: request,
        ),
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    final isStockInsufficient = _quantity > widget.item.currentStock;
    final isLowStock = widget.item.currentStock <= widget.item.minimumStock;

    return Scaffold(
      appBar: AppBar(
        title: const Text('出庫数量入力'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
      ),
      body: Padding(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // 物品情報
            Card(
              child: Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      '物品情報',
                      style: TextStyle(
                        fontSize: 16,
                        fontWeight: FontWeight.bold,
                      ),
                    ),
                    const SizedBox(height: 12),
                    Row(
                      children: [
                        const Text(
                          '物品名: ',
                          style: TextStyle(fontWeight: FontWeight.w500),
                        ),
                        Expanded(
                          child: Text(
                            widget.item.itemName,
                            style: const TextStyle(fontSize: 16),
                          ),
                        ),
                      ],
                    ),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        const Text(
                          'コード: ',
                          style: TextStyle(fontWeight: FontWeight.w500),
                        ),
                        Text(widget.item.itemCode),
                      ],
                    ),
                    const SizedBox(height: 4),
                    Row(
                      children: [
                        const Text(
                          '現在庫: ',
                          style: TextStyle(fontWeight: FontWeight.w500),
                        ),
                        Text(
                          '${widget.item.currentStock} ${widget.item.unit}',
                          style: TextStyle(
                            fontSize: 16,
                            fontWeight: FontWeight.bold,
                            color: isLowStock ? Colors.red : Colors.black,
                          ),
                        ),
                      ],
                    ),
                    if (widget.item.itemDescription != null && widget.item.itemDescription!.isNotEmpty) ...[
                      const SizedBox(height: 4),
                      Text(
                        widget.item.itemDescription!,
                        style: const TextStyle(
                          color: Colors.grey,
                          fontSize: 14,
                        ),
                      ),
                    ],
                  ],
                ),
              ),
            ),
            
            if (isLowStock) ...[
              const SizedBox(height: 8),
              Container(
                width: double.infinity,
                padding: const EdgeInsets.all(12),
                decoration: BoxDecoration(
                  color: Colors.orange.shade50,
                  border: Border.all(color: Colors.orange),
                  borderRadius: BorderRadius.circular(8),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.warning, color: Colors.orange),
                    const SizedBox(width: 8),
                    Expanded(
                      child: Text(
                        '在庫不足です（最小在庫: ${widget.item.minimumStock} ${widget.item.unit}）',
                        style: const TextStyle(color: Colors.orange),
                      ),
                    ),
                  ],
                ),
              ),
            ],

            const SizedBox(height: 24),

            // 数量入力
            const Text(
              '出庫数量',
              style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),
            Row(
              children: [
                IconButton(
                  onPressed: _quantity > 1 ? _decrementQuantity : null,
                  icon: const Icon(Icons.remove),
                  style: IconButton.styleFrom(
                    backgroundColor: Colors.grey.shade200,
                  ),
                ),
                const SizedBox(width: 16),
                SizedBox(
                  width: 100,
                  child: TextField(
                    controller: _quantityController,
                    keyboardType: TextInputType.number,
                    textAlign: TextAlign.center,
                    decoration: const InputDecoration(
                      border: OutlineInputBorder(),
                      contentPadding: EdgeInsets.symmetric(horizontal: 8, vertical: 12),
                    ),
                    onChanged: _onQuantityChanged,
                  ),
                ),
                const SizedBox(width: 16),
                IconButton(
                  onPressed: _incrementQuantity,
                  icon: const Icon(Icons.add),
                  style: IconButton.styleFrom(
                    backgroundColor: Colors.grey.shade200,
                  ),
                ),
                const SizedBox(width: 8),
                Text(widget.item.unit),
              ],
            ),

            if (isStockInsufficient) ...[
              const SizedBox(height: 8),
              Text(
                '在庫が不足しています（現在庫: ${widget.item.currentStock} ${widget.item.unit}）',
                style: const TextStyle(
                  color: Colors.red,
                  fontSize: 14,
                ),
              ),
            ],

            const SizedBox(height: 24),

            // 備考
            const Text(
              '備考（任意）',
              style: TextStyle(
                fontSize: 16,
                fontWeight: FontWeight.bold,
              ),
            ),
            const SizedBox(height: 8),
            TextField(
              controller: _remarksController,
              maxLines: 3,
              decoration: const InputDecoration(
                border: OutlineInputBorder(),
                hintText: '備考があれば入力してください',
              ),
            ),

            const Spacer(),

            // 確認ボタン
            SizedBox(
              width: double.infinity,
              height: 50,
              child: ElevatedButton(
                onPressed: _canProceed ? _proceed : null,
                style: ElevatedButton.styleFrom(
                  backgroundColor: Colors.blue,
                  foregroundColor: Colors.white,
                ),
                child: const Text(
                  '確認画面へ',
                  style: TextStyle(fontSize: 16),
                ),
              ),
            ),
          ],
        ),
      ),
    );
  }

  @override
  void dispose() {
    _quantityController.dispose();
    _remarksController.dispose();
    super.dispose();
  }
}