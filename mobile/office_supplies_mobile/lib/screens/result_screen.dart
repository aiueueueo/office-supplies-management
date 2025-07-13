import 'package:flutter/material.dart';
import '../models/department.dart';
import '../models/item.dart';
import '../services/api_service.dart';
import 'home_screen.dart';
import 'department_selection_screen.dart';

class ResultScreen extends StatefulWidget {
  final bool success;
  final Department department;
  final Item item;
  final int quantity;
  final String? errorMessage;

  const ResultScreen({
    super.key,
    required this.success,
    required this.department,
    required this.item,
    required this.quantity,
    this.errorMessage,
  });

  @override
  State<ResultScreen> createState() => _ResultScreenState();
}

class _ResultScreenState extends State<ResultScreen> {
  final ApiService _apiService = ApiService();
  bool _isCancelling = false;

  Future<void> _cancelTransaction() async {
    final shouldCancel = await showDialog<bool>(
      context: context,
      builder: (context) => AlertDialog(
        title: const Text('取消確認'),
        content: const Text('直前の出庫処理を取り消しますか？\n取り消すと在庫が元に戻ります。'),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(context, false),
            child: const Text('キャンセル'),
          ),
          TextButton(
            onPressed: () => Navigator.pop(context, true),
            child: const Text(
              '取り消す',
              style: TextStyle(color: Colors.red),
            ),
          ),
        ],
      ),
    );

    if (shouldCancel != true) return;

    setState(() {
      _isCancelling = true;
    });

    try {
      final success = await _apiService.cancelLastTransaction();
      
      if (!mounted) return;

      if (success) {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('取引が取り消されました'),
            backgroundColor: Colors.green,
          ),
        );
      } else {
        ScaffoldMessenger.of(context).showSnackBar(
          const SnackBar(
            content: Text('取消に失敗しました'),
            backgroundColor: Colors.red,
          ),
        );
      }
    } catch (e) {
      if (!mounted) return;
      ScaffoldMessenger.of(context).showSnackBar(
        SnackBar(
          content: Text('エラー: $e'),
          backgroundColor: Colors.red,
        ),
      );
    } finally {
      setState(() {
        _isCancelling = false;
      });
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('処理結果'),
        backgroundColor: Theme.of(context).colorScheme.inversePrimary,
        automaticallyImplyLeading: false,
      ),
      body: Padding(
        padding: const EdgeInsets.all(24.0),
        child: Column(
          mainAxisAlignment: MainAxisAlignment.center,
          children: [
            // 結果アイコン
            Icon(
              widget.success ? Icons.check_circle : Icons.error,
              size: 100,
              color: widget.success ? Colors.green : Colors.red,
            ),
            const SizedBox(height: 24),

            // 結果メッセージ
            Text(
              widget.success ? '出庫処理が完了しました' : '出庫処理に失敗しました',
              style: TextStyle(
                fontSize: 24,
                fontWeight: FontWeight.bold,
                color: widget.success ? Colors.green : Colors.red,
              ),
              textAlign: TextAlign.center,
            ),
            const SizedBox(height: 16),

            if (widget.success) ...[
              // 成功時の詳細情報
              Card(
                child: Padding(
                  padding: const EdgeInsets.all(16.0),
                  child: Column(
                    children: [
                      _buildResultRow('部署', widget.department.departmentName),
                      const Divider(),
                      _buildResultRow('物品', widget.item.itemName),
                      const Divider(),
                      _buildResultRow(
                        '出庫数量',
                        '${widget.quantity} ${widget.item.unit}',
                      ),
                      const Divider(),
                      _buildResultRow(
                        '処理後在庫',
                        '${widget.item.currentStock - widget.quantity} ${widget.item.unit}',
                      ),
                    ],
                  ),
                ),
              ),
              const SizedBox(height: 24),

              // 取消ボタン（成功時のみ）
              if (!_isCancelling)
                OutlinedButton(
                  onPressed: _cancelTransaction,
                  style: OutlinedButton.styleFrom(
                    foregroundColor: Colors.red,
                    side: const BorderSide(color: Colors.red),
                    padding: const EdgeInsets.symmetric(
                      horizontal: 24,
                      vertical: 12,
                    ),
                  ),
                  child: const Text('直前の処理を取り消し'),
                )
              else
                const CircularProgressIndicator(),
            ] else ...[
              // エラー時の詳細
              if (widget.errorMessage != null) ...[
                Container(
                  width: double.infinity,
                  padding: const EdgeInsets.all(16),
                  decoration: BoxDecoration(
                    color: Colors.red.shade50,
                    border: Border.all(color: Colors.red.shade200),
                    borderRadius: BorderRadius.circular(8),
                  ),
                  child: Text(
                    widget.errorMessage!,
                    style: const TextStyle(color: Colors.red),
                  ),
                ),
              ],
            ],

            const SizedBox(height: 32),

            // アクションボタン
            Column(
              children: [
                SizedBox(
                  width: double.infinity,
                  height: 50,
                  child: ElevatedButton(
                    onPressed: () {
                      Navigator.pushAndRemoveUntil(
                        context,
                        MaterialPageRoute(
                          builder: (context) => DepartmentSelectionScreen(),
                        ),
                        (route) => route.isFirst,
                      );
                    },
                    style: ElevatedButton.styleFrom(
                      backgroundColor: Colors.blue,
                      foregroundColor: Colors.white,
                    ),
                    child: const Text(
                      '続けて出庫処理',
                      style: TextStyle(fontSize: 16),
                    ),
                  ),
                ),
                const SizedBox(height: 12),
                SizedBox(
                  width: double.infinity,
                  height: 50,
                  child: OutlinedButton(
                    onPressed: () {
                      Navigator.pushAndRemoveUntil(
                        context,
                        MaterialPageRoute(
                          builder: (context) => const HomeScreen(),
                        ),
                        (route) => false,
                      );
                    },
                    child: const Text(
                      'ホームに戻る',
                      style: TextStyle(fontSize: 16),
                    ),
                  ),
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildResultRow(String label, String value) {
    return Padding(
      padding: const EdgeInsets.symmetric(vertical: 4.0),
      child: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          Text(
            label,
            style: const TextStyle(
              fontWeight: FontWeight.w500,
              color: Colors.grey,
            ),
          ),
          Text(
            value,
            style: const TextStyle(
              fontSize: 16,
              fontWeight: FontWeight.w500,
            ),
          ),
        ],
      ),
    );
  }
}