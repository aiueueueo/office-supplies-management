import 'dart:convert';
import 'package:http/http.dart' as http;
import '../models/item.dart';
import '../models/department.dart';
import '../models/transaction_request.dart';

class ApiService {
  // 開発環境のAPIエンドポイント（実際の環境に合わせて変更）
  static const String baseUrl = 'https://localhost:7001/api';
  
  // アイテム情報をバーコードで取得
  Future<Item?> getItemByBarcode(String barcode) async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/items/barcode/$barcode'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final Map<String, dynamic> data = json.decode(response.body);
        return Item.fromJson(data);
      } else if (response.statusCode == 404) {
        return null; // アイテムが見つからない
      } else {
        throw Exception('Failed to load item: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Network error: $e');
    }
  }

  // 部署一覧を取得
  Future<List<Department>> getDepartments() async {
    try {
      final response = await http.get(
        Uri.parse('$baseUrl/departments'),
        headers: {'Content-Type': 'application/json'},
      );

      if (response.statusCode == 200) {
        final List<dynamic> data = json.decode(response.body);
        return data.map((json) => Department.fromJson(json)).toList();
      } else {
        throw Exception('Failed to load departments: ${response.statusCode}');
      }
    } catch (e) {
      throw Exception('Network error: $e');
    }
  }

  // 出庫処理を実行
  Future<bool> processStockOut(TransactionRequest request) async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/transactions/stock-out'),
        headers: {'Content-Type': 'application/json'},
        body: json.encode(request.toJson()),
      );

      return response.statusCode == 200 || response.statusCode == 201;
    } catch (e) {
      throw Exception('Network error: $e');
    }
  }

  // 直前の取引を取り消し
  Future<bool> cancelLastTransaction() async {
    try {
      final response = await http.post(
        Uri.parse('$baseUrl/transactions/cancel-last'),
        headers: {'Content-Type': 'application/json'},
      );

      return response.statusCode == 200;
    } catch (e) {
      throw Exception('Network error: $e');
    }
  }
}