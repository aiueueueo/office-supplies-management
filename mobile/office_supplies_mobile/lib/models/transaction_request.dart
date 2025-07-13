class TransactionRequest {
  final int itemId;
  final int departmentId;
  final int quantity;
  final String? remarks;
  final String processedBy;

  TransactionRequest({
    required this.itemId,
    required this.departmentId,
    required this.quantity,
    this.remarks,
    required this.processedBy,
  });

  Map<String, dynamic> toJson() {
    return {
      'itemId': itemId,
      'departmentId': departmentId,
      'quantity': quantity,
      'remarks': remarks,
      'processedBy': processedBy,
    };
  }
}