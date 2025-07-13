class Department {
  final int departmentId;
  final String departmentCode;
  final String departmentName;
  final bool isActive;

  Department({
    required this.departmentId,
    required this.departmentCode,
    required this.departmentName,
    required this.isActive,
  });

  factory Department.fromJson(Map<String, dynamic> json) {
    return Department(
      departmentId: json['departmentId'],
      departmentCode: json['departmentCode'],
      departmentName: json['departmentName'],
      isActive: json['isActive'],
    );
  }

  Map<String, dynamic> toJson() {
    return {
      'departmentId': departmentId,
      'departmentCode': departmentCode,
      'departmentName': departmentName,
      'isActive': isActive,
    };
  }
}