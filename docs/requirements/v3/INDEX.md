# v3 Screen Requirements Index

> **Pack guide:** [README.md](README.md) — deep-dive index and rebuild conventions.


Migration-ready requirements for rebuilding Ozone ERP / CPZSAS from the **v3** CodeIgniter app.

- Source: `v3/`
- Template: [_template.md](_template.md)
- Inventory CSV: [_inventory.csv](_inventory.csv)
- Total menu entries: 294 (pages with URL: 262; nav folders: 32)

## Status legend

| Status | Meaning |
|--------|---------|
| done | Detailed requirements file written |
| folder | Navigation parent only (no page file) |

## Waves

| Wave | Focus | Priority |
|------|-------|----------|
| 1 | Core ERP (auth, masters, sales, purchase, vouchers, stock, users/settings) | P0 |
| 2 | POS, Reports, CRM, HR, Analytics | P1 |
| 3 | Clinic, Education, Hotel, Gold, Church, Transport, Production, Ecommerce | P1/P2 |
| 4 | REST / mobile APIs | P1 for mobile/ecom parity |

## Wave 1 — Core ERP

| ID | Screen | Folder | Spec file | Legacy URL | Status |
|----|--------|--------|-----------|------------|--------|
| 9 | Members | other | [other/members.md](other/members.md) | `main/members/listall` | done |
| 10 | Member Category | other | [other/member-category.md](other/member-category.md) | `masters/member_category` | done |
| 11 | Basic Units | master | [master/basic-units.md](master/basic-units.md) | `BasicUnits/listall` | done |
| 12 | Suppliers (Sundry Creditors) | master | [master/suppliers-sundry-creditors.md](master/suppliers-sundry-creditors.md) | `suppliers/listall/sup` | done |
| 17 | Company Info | master | [master/company-info.md](master/company-info.md) | `company/com_info/listall` | done |
| 19 | Sub Account | accounts_master | [accounts_master/sub-account.md](accounts_master/sub-account.md) | `accounts/accsubtype` | done |
| 20 | Main Account | accounts_master | [accounts_master/main-account.md](accounts_master/main-account.md) | `accounts/acctype` | done |
| 21 | Ledger Group | accounts_master | [accounts_master/ledger-group.md](accounts_master/ledger-group.md) | `accounts_master/LedgerGroup/listall` | done |
| 22 | Ledgers | accounts_master | [accounts_master/ledgers.md](accounts_master/ledgers.md) | `accounts_master/Ledger/listall` | done |
| 25 | Item Type | master | [master/item-type.md](master/item-type.md) | `items//itemtype` | done |
| 26 | Category | master | [master/category.md](master/category.md) | `items//itemcategory` | done |
| 30 | Payment Vouchers | transaction | [transaction/payment-vouchers.md](transaction/payment-vouchers.md) | `transaction//vouchers//listall//payment` | done |
| 31 | Journal Vouchers | transaction | [transaction/journal-vouchers.md](transaction/journal-vouchers.md) | `transaction//vouchers//listall//journal` | done |
| 32 | Receipt Vouchers | transaction | [transaction/receipt-vouchers.md](transaction/receipt-vouchers.md) | `transaction//vouchers//listall//receipt` | done |
| 33 | Purchase Register | transaction | [transaction/purchase-register.md](transaction/purchase-register.md) | `transaction/purchase/listall/purchase` | done |
| 35 | Member Type | other | [other/member-type.md](other/member-type.md) | `masters/member_type` | done |
| 44 | Sales Order | transaction | [transaction/sales-order.md](transaction/sales-order.md) | `transaction/sales/listall/estimate` | done |
| 45 | Reference (Agent) | other | [other/reference-agent.md](other/reference-agent.md) | `masters/reference` | done |
| 46 | Customers (Sundry Debtor) | master | [master/customers-sundry-debtor.md](master/customers-sundry-debtor.md) | `customers/listall` | done |
| 48 | Sales Register | transaction | [transaction/sales-register.md](transaction/sales-register.md) | `transaction/sales/listall/sales` | done |
| 51 | Menus | settings | [settings/menus.md](settings/menus.md) | `masters/menu` | done |
| 52 | Sales Return | transaction | [transaction/sales-return.md](transaction/sales-return.md) | `transaction/sales/listall/sales_return` | done |
| 54 | Db Download | settings | [settings/db-download.md](settings/db-download.md) | `settings/setting/db_download` | done |
| 55 | Item Discount Group | master | [master/item-discount-group.md](master/item-discount-group.md) | `items//itemgroup` | done |
| 58 | Item Master | master | [master/item-master.md](master/item-master.md) | `inventory_master/Item/listall` | done |
| 60 | Messages | settings | [settings/messages.md](settings/messages.md) | `settings/sms/msg` | done |
| 87 | GST | other | [other/gst.md](other/gst.md) | `masters/gst` | done |
| 88 | Hsn Code | other | [other/hsn-code.md](other/hsn-code.md) | `masters/hsncode` | done |
| 91 | Software Version | settings | [settings/software-version.md](settings/software-version.md) | `settings/SoftwareVersion/listall` | done |
| 92 | Users | master | [master/users.md](master/users.md) | `settings/NewMaster/Users` | done |
| 95 | Company Settings | settings | [settings/company-settings.md](settings/company-settings.md) | `settings/CompanySettings/listall` | done |
| — | Company Profile / Address | settings | [settings/company-profile.md](settings/company-profile.md) | OzoneAI tenant profile | done |
| — | Financial Year | settings | [settings/financial-year.md](settings/financial-year.md) | OzoneAI FY switch / close | done |
| 97 | Menu To User | settings | [settings/menu-to-user.md](settings/menu-to-user.md) | `settings/MenuToUser/listall` | done |
| 105 | Purchase Return | transaction | [transaction/purchase-return.md](transaction/purchase-return.md) | `transaction/purchase/listall/purchase_return` | done |
| 106 | Purchase Order | transaction | [transaction/purchase-order.md](transaction/purchase-order.md) | `transaction/purchase/listall/order` | done |
| 108 | Bill Types | master | [master/bill-types.md](master/bill-types.md) | `master/Newmasters/bill_type` | done |
| 109 | Area | master | [master/area.md](master/area.md) | `master/Newmasters/Area` | done |
| 111 | Print Types | master | [master/print-types.md](master/print-types.md) | `master/Newmasters/print_type` | done |
| 112 | Excel Import | master | [master/excel-import.md](master/excel-import.md) | `item/excel` | done |
| 124 | Godown Transfer | master | [master/godown-transfer.md](master/godown-transfer.md) | `master/itemtransfer/list_all` | done |
| 129 | Set Device | settings | [settings/set-device.md](settings/set-device.md) | `settings/set_device/device` | done |
| 131 | Godown | master | [master/godown.md](master/godown.md) | `master/godown/godown` | done |
| 132 | Racks | master | [master/racks.md](master/racks.md) | `Item/racks` | done |
| 133 | Login Logs | settings | [settings/login-logs.md](settings/login-logs.md) | `settings/loginsett/listall` | done |
| 191 | Damage Stock | master | [master/damage-stock.md](master/damage-stock.md) | `master/Itemdamage/list_all` | done |
| 194 | My Menu | settings | [settings/my-menu.md](settings/my-menu.md) | `settings/menus/user_menu` | done |
| 198 | Material Type | master | [master/material-type.md](master/material-type.md) | `master/amaster/material_type` | done |
| 204 | Currency | master | [master/currency.md](master/currency.md) | `master/amaster/currency_type` | done |
| 207 | Coupons | master | [master/coupons.md](master/coupons.md) | `master/coupons/listall` | done |
| 210 | Route | master | [master/route.md](master/route.md) | `master/newmasters/route` | done |
| 211 | Sales Bill | transaction | [transaction/sales-bill.md](transaction/sales-bill.md) | `transaction/sales/sales/sales` | done |
| 212 | Purchase Bill | transaction | [transaction/purchase-bill.md](transaction/purchase-bill.md) | `transaction/purchase/create/purchase` | done |
| 213 | Opening Stock Edit | master | [master/opening-stock-edit.md](master/opening-stock-edit.md) | `itemedit/listall` | done |
| 214 | Set Database | master | [master/set-database.md](master/set-database.md) | `master/Newmasters/year_database` | done |
| 216 | Brands | master | [master/brands.md](master/brands.md) | `master/amaster/brands_master` | done |
| 218 | Printers | master | [master/printers.md](master/printers.md) | `master/amaster/printers` | done |
| 220 | Cess | other | [other/cess.md](other/cess.md) | `masters/cess` | done |
| 222 | User Settings | master | [master/user-settings.md](master/user-settings.md) | `settings/NewMaster/user_settings` | done |
| 226 | Bill Format | settings | [settings/bill-format.md](settings/bill-format.md) | `billformat/billformat` | done |
| 229 | Slider | settings | [settings/slider.md](settings/slider.md) | `settings/sliders/listall` | done |
| 230 | Cheque | transaction | [transaction/cheque.md](transaction/cheque.md) | `transaction/chque/list_all` | done |
| 231 | Bank | other | [other/bank.md](other/bank.md) | `Masters/bank` | done |
| 241 | SMS | settings | [settings/sms.md](settings/sms.md) | `settings/message/listall` | done |
| 242 | SMS Types | settings | [settings/sms-types.md](settings/sms-types.md) | `settings/sms/sms_types` | done |
| 243 | SMS Settings | settings | [settings/sms-settings.md](settings/sms-settings.md) | `settings/sms/sms_settings` | done |
| 246 | Quick Voucher | transaction | [transaction/quick-voucher.md](transaction/quick-voucher.md) | `transaction/Quick_voucher/listall` | done |
| 251 | Merge | other | [other/merge.md](other/merge.md) | `Merge/Ledger` | done |
| 256 | Tags | other | [other/tags.md](other/tags.md) | `Item_tags/list_all` | done |
| 257 | Item Tags | other | [other/item-tags.md](other/item-tags.md) | `Item_tags/list_tag` | done |
| 265 | Vehicle Sales | transport | [transport/vehicle-sales.md](transport/vehicle-sales.md) | `sales_vehicle/Sales_vehicle/listall` | done |
| 338 | Quotation | transaction | [transaction/quotation.md](transaction/quotation.md) | `transaction/Quotation/listall` | done |
| 340 | Price List | master | [master/price-list.md](master/price-list.md) | `pricelist/Pricelist/listall` | done |
| 341 | Modify Item Rates | master | [master/modify-item-rates.md](master/modify-item-rates.md) | `Itemsedit/listall` | done |
| 343 | Branch | other | [other/branch.md](other/branch.md) | `Masters/company_branch` | done |
| 345 | Payment List | transaction | [transaction/payment-list.md](transaction/payment-list.md) | `transaction//vouchers//list_all//payment` | done |
| 346 | Receipt List | transaction | [transaction/receipt-list.md](transaction/receipt-list.md) | `transaction//vouchers//list_all//receipt` | done |
| 348 | Set Free Quantity | master | [master/set-free-quantity.md](master/set-free-quantity.md) | `itemedit/list` | done |
| 349 | Contra | transaction | [transaction/contra.md](transaction/contra.md) | `transaction//vouchers//listall//contra` | done |
| 351 | Company Keys | settings | [settings/company-keys.md](settings/company-keys.md) | `settings/CompanySettings/company_keys` | done |
| 361 | Delivery Note | transaction | [transaction/delivery-note.md](transaction/delivery-note.md) | `transaction/sales/listall/delivery_note` | done |
| 387 | Districts | other | [other/districts.md](other/districts.md) | `masters/states` | done |
| 388 | Source Of Enquiry | other | [other/source-of-enquiry.md](other/source-of-enquiry.md) | `source_of_enquiry/Source_of_enquiry/list_all` | done |
| 420 | Stock Journal | master | [master/stock-journal.md](master/stock-journal.md) | `Stock_journal/listall` | done |
| 432 | Vehicle | master | [master/vehicle.md](master/vehicle.md) | `master/Newmasters/ims_vehicle` | done |
| 439 | Physical Stock Update | master | [master/physical-stock-update.md](master/physical-stock-update.md) | `Physical_stock/list` | done |
| 446 | UQC | master | [master/uqc.md](master/uqc.md) | `master/Newmasters/uqc` | done |
| 449 | Point Settings | other | [other/point-settings.md](other/point-settings.md) | `Masters/points` | done |
| 454 | Replacement voucher | master | [master/replacement-voucher.md](master/replacement-voucher.md) | `Replace_items/list` | done |
| 455 | Districts | master | [master/districts.md](master/districts.md) | `master/Newmasters/district` | done |
| 456 | Customer Type | master | [master/customer-type.md](master/customer-type.md) | `master/Newmasters/customer_type` | done |
| 476 | Leave Type | master | [master/leave-type.md](master/leave-type.md) | `master/Newmasters/hr_leavetype` | done |
| 479 | Debit Note | transaction | [transaction/debit-note.md](transaction/debit-note.md) | `transaction/Credit_Debit_Vouchers/list_all/debit_note` | done |
| 480 | Credit Note | transaction | [transaction/credit-note.md](transaction/credit-note.md) | `transaction/Credit_Debit_Vouchers/list_all` | done |
| 491 | Admin Dashboard | dashboard | [dashboard/admin-dashboard.md](dashboard/admin-dashboard.md) | `dashboard/summary` | done |
| 495 | Change Item Tax | master | [master/change-item-tax.md](master/change-item-tax.md) | `inventory_master/Item/change_item_tax` | done |

## Wave 2 — Ops

| ID | Screen | Folder | Spec file | Legacy URL | Status |
|----|--------|--------|-----------|------------|--------|
| 37 | Ledger Book | accounts_master | [accounts_master/ledger-book.md](accounts_master/ledger-book.md) | `reports/accounts/leadger` | done |
| 41 | Cash Book | accounts_master | [accounts_master/cash-book.md](accounts_master/cash-book.md) | `reports/accounts/cashbook` | done |
| 42 | Day Book | accounts_master | [accounts_master/day-book.md](accounts_master/day-book.md) | `reports/accounts/daybook` | done |
| 43 | Trial Balance | accounts_master | [accounts_master/trial-balance.md](accounts_master/trial-balance.md) | `reports/accounts/trial` | done |
| 49 | Stock Ledger | reports | [reports/stock-ledger.md](reports/stock-ledger.md) | `reports/stock/leadger` | done |
| 53 | Check List | accounts_master | [accounts_master/check-list.md](accounts_master/check-list.md) | `reports/accounts/checklist` | done |
| 57 | Discount Group  Report | reports | [reports/discount-group-report.md](reports/discount-group-report.md) | `reports/sales/type` | done |
| 89 | Aging | reports | [reports/aging.md](reports/aging.md) | `reports/aging/listall` | done |
| 118 | Item Wise Sales Register | master | [master/item-wise-sales-register.md](master/item-wise-sales-register.md) | `reports/item/itemereport` | done |
| 121 | GSTR 1 | reports | [reports/gstr-1.md](reports/gstr-1.md) | `reports/gst/gstreport` | done |
| 122 | GSTR 2 Report | reports | [reports/gstr-2-report.md](reports/gstr-2-report.md) | `reports/gst/gstr2report` | done |
| 125 | Purchase Report | reports | [reports/purchase-report.md](reports/purchase-report.md) | `reports/purchase/purchasereport` | done |
| 126 | Sales Reports | reports | [reports/sales-reports.md](reports/sales-reports.md) | `reports/Sales/salesreport` | done |
| 156 | Item Wise Sales Report | reports | [reports/item-wise-sales-report.md](reports/item-wise-sales-report.md) | `reports/itemwisereport/sales` | done |
| 157 | Opening Stock Report | reports | [reports/opening-stock-report.md](reports/opening-stock-report.md) | `reports/itemdetails/listall` | done |
| 159 | Item Wise Margin Report | reports | [reports/item-wise-margin-report.md](reports/item-wise-margin-report.md) | `reports/marginreport/itemwise` | done |
| 160 | BillWise Margin Report | reports | [reports/billwise-margin-report.md](reports/billwise-margin-report.md) | `reports/marginreport/billwise` | done |
| 161 | Stock Movement Report | reports | [reports/stock-movement-report.md](reports/stock-movement-report.md) | `reports/itemwisereport/quantity` | done |
| 162 | Total Sales Report | reports | [reports/total-sales-report.md](reports/total-sales-report.md) | `reports/totalinvoicereport/sales` | done |
| 163 | Total Purchase Report | reports | [reports/total-purchase-report.md](reports/total-purchase-report.md) | `reports/totalinvoicereport/purchase` | done |
| 165 | Item Wise Purchase Report | reports | [reports/item-wise-purchase-report.md](reports/item-wise-purchase-report.md) | `reports/itemwisereport/purchase` | done |
| 166 | Invoice Report | reports | [reports/invoice-report.md](reports/invoice-report.md) | `reports/invoice/invoicereport` | done |
| 199 | Profit & Loss A/C | accounts_master | [accounts_master/profit-loss-ac.md](accounts_master/profit-loss-ac.md) | `reports/accounts/profitandlossdaywise` | done |
| 200 | Purchase Return Report | reports | [reports/purchase-return-report.md](reports/purchase-return-report.md) | `reports/purchase/purchasereturn` | done |
| 201 | Sales Order Report | reports | [reports/sales-order-report.md](reports/sales-order-report.md) | `reports/Sales/saleorder` | done |
| 203 | Sales Return Report | reports | [reports/sales-return-report.md](reports/sales-return-report.md) | `reports/Sales/salesreturn` | done |
| 205 | Total Outstanding Report | reports | [reports/total-outstanding-report.md](reports/total-outstanding-report.md) | `reports/outstandingreport/outstandingreport` | done |
| 206 | Outstanding Reports | reports | [reports/outstanding-reports.md](reports/outstanding-reports.md) | `reports/aging/outstand` | done |
| 223 | Stock Report | master | [master/stock-report.md](master/stock-report.md) | `reports/Items/listall` | done |
| 224 | Sales Order Merge Report | reports | [reports/sales-order-merge-report.md](reports/sales-order-merge-report.md) | `reports/SalesOrderMerge/mergereport` | done |
| 232 | Customer Ledger Balance | reports | [reports/customer-ledger-balance.md](reports/customer-ledger-balance.md) | `reports/Ledger_balance/ledger_balance/cus` | done |
| 233 | Supplier Ledger Balance | reports | [reports/supplier-ledger-balance.md](reports/supplier-ledger-balance.md) | `reports/Ledger_balance/ledger_balance/sup` | done |
| 235 | Detailed stock report | master | [master/detailed-stock-report.md](master/detailed-stock-report.md) | `reports/items/list_all` | done |
| 244 | Day Summary Report | reports | [reports/day-summary-report.md](reports/day-summary-report.md) | `reports/Daybook/Daybook` | done |
| 245 | Commission Report | reports | [reports/commission-report.md](reports/commission-report.md) | `reports/Commission/Commision` | done |
| 247 | Day Report | reports | [reports/day-report.md](reports/day-report.md) | `reports/Daybook/Day_Report` | done |
| 252 | Corrupted Ledger | reports | [reports/corrupted-ledger.md](reports/corrupted-ledger.md) | `reports/Ledger_mistake/ledger_mistake` | done |
| 305 | Salary Distribution | hr | [hr/salary-distribution.md](hr/salary-distribution.md) | `hr/SalaryDistribution/list_all` | done |
| 306 | Salary Package | hr | [hr/salary-package.md](hr/salary-package.md) | `hr/SalaryPackage/list_all` | done |
| 309 | Designation | hr | [hr/designation.md](hr/designation.md) | `hr/designation/list_all` | done |
| 310 | Department | hr | [hr/department.md](hr/department.md) | `hr/department/list_all` | done |
| 311 | Role | hr | [hr/role.md](hr/role.md) | `hr/role/list_all` | done |
| 312 | Holidays | hr | [hr/holidays.md](hr/holidays.md) | `hr/Holidays/list_all` | done |
| 313 | Pay Settings | settings | [settings/pay-settings.md](settings/pay-settings.md) | `hr/paysettings/list_all` | done |
| 314 | Leave Type | hr | [hr/leave-type.md](hr/leave-type.md) | `hr/leavetype/list_all` | done |
| 315 | Course | hr | [hr/course.md](hr/course.md) | `hr/course/list_all` | done |
| 316 | Employee | hr | [hr/employee.md](hr/employee.md) | `hr/Employee/list_all` | done |
| 318 | Lead report | reports | [reports/lead-report.md](reports/lead-report.md) | `crm_general/reports/Lead_report/list_leads_report` | done |
| 319 | Enquiry report | reports | [reports/enquiry-report.md](reports/enquiry-report.md) | `crm_general/reports/Enquiry_report/list_enquiry_report` | done |
| 320 | Productwise Enquiry report | reports | [reports/productwise-enquiry-report.md](reports/productwise-enquiry-report.md) | `crm_general/reports/Productwise_report/list_product_report` | done |
| 321 | Followup history report | reports | [reports/followup-history-report.md](reports/followup-history-report.md) | `crm_general/reports/Followup_report/list_followup_report` | done |
| 322 | Enquiry completion report | reports | [reports/enquiry-completion-report.md](reports/enquiry-completion-report.md) | `crm_general/reports/Enquiry_completion_report/list_completion_report` | done |
| 323 | Allocated Enquiry Report | reports | [reports/allocated-enquiry-report.md](reports/allocated-enquiry-report.md) | `crm_general/reports/Allocated_report/list_allocated_report` | done |
| 324 | Business Opportunity Report | reports | [reports/business-opportunity-report.md](reports/business-opportunity-report.md) | `crm_general/reports/Bussiness_opportunity_report/list_opportunity_report` | done |
| 325 | Business plan report | reports | [reports/business-plan-report.md](reports/business-plan-report.md) | `crm_general/reports/Bussiness_plan_report/list_busiplan_report` | done |
| 326 | Leads Master | master | [master/leads-master.md](master/leads-master.md) | `crm_general/Leads_master/listall` | done |
| 327 | Enquiry Register | crm | [crm/enquiry-register.md](crm/enquiry-register.md) | `crm_general/Enquiry_register/list_all` | done |
| 328 | Enquiry Master | crm | [crm/enquiry-master.md](crm/enquiry-master.md) | `crm_general/Enquiry_allocation/list_all` | done |
| 330 | Enquiry Master | crm | [crm/enquiry-master.md](crm/enquiry-master.md) | `crm_new/Enquiry_allocation/list_all` | done |
| 331 | Enquiry Register | crm | [crm/enquiry-register.md](crm/enquiry-register.md) | `crm_new/Enquiry_register/list_all` | done |
| 333 | Type of Bussiness | master | [master/type-of-bussiness.md](master/type-of-bussiness.md) | `crm_general/Leads_master/listTypeofBuss` | done |
| 334 | Nature of Bussiness | crm | [crm/nature-of-bussiness.md](crm/nature-of-bussiness.md) | `crm_general/Enquiry_allocation/listOtherofBuss` | done |
| 339 | Status Master | crm | [crm/status-master.md](crm/status-master.md) | `crm_general/Enquiry_followup/listStatus` | done |
| 342 | POS | pos | [pos/pos.md](pos/pos.md) | `pos/Main/listall` | done |
| 344 | Cost Center | analytics | [analytics/cost-center.md](analytics/cost-center.md) | `cost_analytics/Cost_centre/listall` | done |
| 347 | Sales Cash Report | reports | [reports/sales-cash-report.md](reports/sales-cash-report.md) | `reports/invoice/salescashreport` | done |
| 350 | Sales Summary Report | reports | [reports/sales-summary-report.md](reports/sales-summary-report.md) | `reports/Sales/sales_summary` | done |
| 353 | Service Call Register | crm | [crm/service-call-register.md](crm/service-call-register.md) | `crm_service/Enquiry_allocation/list_all` | done |
| 354 | Allocated Services | crm | [crm/allocated-services.md](crm/allocated-services.md) | `crm_service/Enquiry_register/list_all` | done |
| 355 | Service Bill | crm | [crm/service-bill.md](crm/service-bill.md) | `crm_service/Enquiry_list/listall` | done |
| 356 | Area Wise Item Report | reports | [reports/area-wise-item-report.md](reports/area-wise-item-report.md) | `reports/area/Itemwise` | done |
| 357 | Route Customer Bill Details | reports | [reports/route-customer-bill-details.md](reports/route-customer-bill-details.md) | `reports/area/Customerwise` | done |
| 358 | Hsn Wise Report | reports | [reports/hsn-wise-report.md](reports/hsn-wise-report.md) | `reports/hsn/Hsnreport` | done |
| 359 | Item Wise Hsn Report | reports | [reports/item-wise-hsn-report.md](reports/item-wise-hsn-report.md) | `reports/hsn/itemreport` | done |
| 360 | E-way Bill Report | reports | [reports/e-way-bill-report.md](reports/e-way-bill-report.md) | `reports/hsn/eway` | done |
| 368 | Balance Sheet | accounts_master | [accounts_master/balance-sheet.md](accounts_master/balance-sheet.md) | `reports/accounts/balancesheet` | done |
| 369 | Ledger Group report | accounts_master | [accounts_master/ledger-group-report.md](accounts_master/ledger-group-report.md) | `reports/accounts/mainLeadger` | done |
| 376 | Order Items Total | master | [master/order-items-total.md](master/order-items-total.md) | `reports/items/order_items` | done |
| 377 | Multi Unit Stock | master | [master/multi-unit-stock.md](master/multi-unit-stock.md) | `reports/items/unit_stock` | done |
| 385 | Label Master | crm | [crm/label-master.md](crm/label-master.md) | `crm_new/Enquiry_followup/listLabel` | done |
| 389 | Address report | reports | [reports/address-report.md](reports/address-report.md) | `reports/Address_report/index` | done |
| 390 | Customer Balance | reports | [reports/customer-balance.md](reports/customer-balance.md) | `reports/Invoice/customercashreport` | done |
| 391 | Itemwise Order | reports | [reports/itemwise-order.md](reports/itemwise-order.md) | `reports/itemwisereport/salesorder` | done |
| 392 | Item Wise Order Register | master | [master/item-wise-order-register.md](master/item-wise-order-register.md) | `reports/item/itemereport/estimate` | done |
| 397 | Jou Voucher Report | reports | [reports/jou-voucher-report.md](reports/jou-voucher-report.md) | `reports/New_Voucher_Report/listall` | done |
| 400 | Stock Level Report | master | [master/stock-level-report.md](master/stock-level-report.md) | `reports/Item/stocklevel` | done |
| 401 | Closing Stock Report | master | [master/closing-stock-report.md](master/closing-stock-report.md) | `reports/items/fifo_lifo` | done |
| 402 | Customer Sales Report | reports | [reports/customer-sales-report.md](reports/customer-sales-report.md) | `reports/invoice/customereisereport` | done |
| 403 | GST EXPORT | reports | [reports/gst-export.md](reports/gst-export.md) | `reports/GST1Report/list` | done |
| 404 | Stock Movment Report | reports | [reports/stock-movment-report.md](reports/stock-movment-report.md) | `reports/Stock_movement_report/list` | done |
| 407 | Pay Voucher Report | reports | [reports/pay-voucher-report.md](reports/pay-voucher-report.md) | `reports/New_voucher_payment/listall` | done |
| 408 | Rec Voucher Report | reports | [reports/rec-voucher-report.md](reports/rec-voucher-report.md) | `reports/New_voucher_receipt/listall` | done |
| 409 | Sale Voucher Report | reports | [reports/sale-voucher-report.md](reports/sale-voucher-report.md) | `reports/New_Sales_report/listall` | done |
| 410 | Pur Voucher Report | reports | [reports/pur-voucher-report.md](reports/pur-voucher-report.md) | `reports/New_Purchase_report/listall` | done |
| 411 | Contra Voucher Report | reports | [reports/contra-voucher-report.md](reports/contra-voucher-report.md) | `reports/New_voucher_contra/listall` | done |
| 414 | Gst purchase tax wise | reports | [reports/gst-purchase-tax-wise.md](reports/gst-purchase-tax-wise.md) | `reports/hsn/Billwise_purchase` | done |
| 415 | Gst sales tax wise | reports | [reports/gst-sales-tax-wise.md](reports/gst-sales-tax-wise.md) | `reports/hsn/Billwise` | done |
| 419 | Route Summary Report | reports | [reports/route-summary-report.md](reports/route-summary-report.md) | `reports/sales/customersales` | done |
| 422 | Monthly Report | reports | [reports/monthly-report.md](reports/monthly-report.md) | `reports/MIS/Monthly_summary` | done |
| 423 | Daybook Custome | reports | [reports/daybook-custome.md](reports/daybook-custome.md) | `reports/Daybook/daybook_custome` | done |
| 424 | GST Sales Day Summary | reports | [reports/gst-sales-day-summary.md](reports/gst-sales-day-summary.md) | `reports/hsn/Daywise` | done |
| 425 | Import | crm | [crm/import.md](crm/import.md) | `crm_general/Excel_import/excel` | done |
| 428 | Stock Journal Report | transaction | [transaction/stock-journal-report.md](transaction/stock-journal-report.md) | `reports/ItemTransaction/stock_journal` | done |
| 429 | Godown Transfer Report | transaction | [transaction/godown-transfer-report.md](transaction/godown-transfer-report.md) | `reports/ItemTransaction/godown_transfer` | done |
| 430 | UserWise Collection | reports | [reports/userwise-collection.md](reports/userwise-collection.md) | `reports/MIS/route_summary` | done |
| 434 | AVG Item Margin | reports | [reports/avg-item-margin.md](reports/avg-item-margin.md) | `reports/marginreport/custom_report` | done |
| 436 | SalesMan performance | reports | [reports/salesman-performance.md](reports/salesman-performance.md) | `reports/MIS/agent_summary` | done |
| 437 | Stock Movement | transaction | [transaction/stock-movement.md](transaction/stock-movement.md) | `reports/ItemTransaction/item_movement` | done |
| 438 | HsnWise Quantity Report | reports | [reports/hsnwise-quantity-report.md](reports/hsnwise-quantity-report.md) | `reports/hsn/Hsnreport_new` | done |
| 440 | Physical Stock Report | transaction | [transaction/physical-stock-report.md](transaction/physical-stock-report.md) | `reports/ItemTransaction/physical_stk_update` | done |
| 445 | Sales HSN REPORT | reports | [reports/sales-hsn-report.md](reports/sales-hsn-report.md) | `reports/hsn/saleshsn` | done |
| 447 | Landing Cost Margin Report | reports | [reports/landing-cost-margin-report.md](reports/landing-cost-margin-report.md) | `reports/Marginreport/landing_cost` | done |
| 448 | Sales Exchange Report\n | reports | [reports/sales-exchange-reportn.md](reports/sales-exchange-reportn.md) | `reports/Salesreturn/Adjustment` | done |
| 450 | Item Wise SR Register | master | [master/item-wise-sr-register.md](master/item-wise-sr-register.md) | `reports/item/itemereport/sales_return` | done |
| 451 | Item Wise SR Report | reports | [reports/item-wise-sr-report.md](reports/item-wise-sr-report.md) | `reports/itemwisereport/sales/sales_return` | done |
| 452 | Purchase Hsn Report | reports | [reports/purchase-hsn-report.md](reports/purchase-hsn-report.md) | `reports/hsn/purchasehsn` | done |
| 453 | Customer Point Report | reports | [reports/customer-point-report.md](reports/customer-point-report.md) | `reports/Address_report/Reedeem` | done |
| 457 | Sales Forecast Report | reports | [reports/sales-forecast-report.md](reports/sales-forecast-report.md) | `reports/invoice/sales_forecast` | done |
| 463 | Godown Stock Register | reports | [reports/godown-stock-register.md](reports/godown-stock-register.md) | `reports/Stock/day_stock` | done |
| 464 | Agewise Debtors Report | reports | [reports/agewise-debtors-report.md](reports/agewise-debtors-report.md) | `reports/Ledger_balance/period_ledger_balance` | done |
| 465 | P Order Pending | reports | [reports/p-order-pending.md](reports/p-order-pending.md) | `reports/Purchase_order_pending/list` | done |
| 466 | Sales Order Pending | reports | [reports/sales-order-pending.md](reports/sales-order-pending.md) | `reports/Sales_order_pending/list` | done |
| 467 | GST SUMMARY | reports | [reports/gst-summary.md](reports/gst-summary.md) | `reports/GstSummary/gstsummary` | done |
| 470 | Cheque Collection Report | reports | [reports/cheque-collection-report.md](reports/cheque-collection-report.md) | `reports/Cheque/Cheque_data` | done |
| 471 | Salary Report | hr | [hr/salary-report.md](hr/salary-report.md) | `hr/SalaryReport/salary_report` | done |
| 472 | Employee Report | hr | [hr/employee-report.md](hr/employee-report.md) | `hr/EmployeeReport/list_all` | done |
| 474 | Attendance | hr | [hr/attendance.md](hr/attendance.md) | `hr/Attendance/list_all` | done |
| 475 | Employee Leave | hr | [hr/employee-leave.md](hr/employee-leave.md) | `hr/EmployeeLeave/list_all` | done |
| 477 | B2b Sales Report | reports | [reports/b2b-sales-report.md](reports/b2b-sales-report.md) | `reports/Gst/b2breport` | done |
| 478 | B2c Sales Report | reports | [reports/b2c-sales-report.md](reports/b2c-sales-report.md) | `reports/Gst/b2creport` | done |
| 481 | Hsn sales | reports | [reports/hsn-sales.md](reports/hsn-sales.md) | `reports/hsn/saleshsn_consolidate` | done |
| 482 | Item List Report | master | [master/item-list-report.md](master/item-list-report.md) | `reports/item/list_item` | done |
| 486 | GSTR2 Export | reports | [reports/gstr2-export.md](reports/gstr2-export.md) | `reports/GST1Report/list_purchase` | done |
| 492 | GSTR1 Export | reports | [reports/gstr1-export.md](reports/gstr1-export.md) | `reports/GST1Report/list_sales` | done |
| 493 | Item Group sales Report | reports | [reports/item-group-sales-report.md](reports/item-group-sales-report.md) | `reports/Brandreport/brand_category` | done |
| 494 | Brand Sales Analysis | reports | [reports/brand-sales-analysis.md](reports/brand-sales-analysis.md) | `reports/Brandreport/brand_sales` | done |
| 496 | Cancelled | reports | [reports/cancelled.md](reports/cancelled.md) | `reports/Sales/salesreport/cancelled` | done |
| 497 | Item Wise PR Report | reports | [reports/item-wise-pr-report.md](reports/item-wise-pr-report.md) | `reports/itemwisereport/purchase/purchase_return` | done |
| 498 | Item Wise PR Register | master | [master/item-wise-pr-register.md](master/item-wise-pr-register.md) | `reports/item/pitemereport/purchase_return` | done |
| 499 | Item Wise Purchase Register | master | [master/item-wise-purchase-register.md](master/item-wise-purchase-register.md) | `reports/item/pitemereport` | done |

## Wave 3 — Verticals

| ID | Screen | Folder | Spec file | Legacy URL | Status |
|----|--------|--------|-----------|------------|--------|
| 63 | Districts | other | [other/districts.md](other/districts.md) | `blood/blood/districts` | done |
| 260 | Vehicle | transport | [transport/vehicle.md](transport/vehicle.md) | `Vehicle_masters/vehicle` | done |
| 261 | Insurance | transport | [transport/insurance.md](transport/insurance.md) | `Vehicle_masters/vehicle_insurance` | done |
| 262 | Insurance Company | transport | [transport/insurance-company.md](transport/insurance-company.md) | `Vehicle_masters/vehicle_insurance_company` | done |
| 263 | Manufactur | transport | [transport/manufactur.md](transport/manufactur.md) | `Vehicle_masters/vehicle_manufacture` | done |
| 264 | Drivers | transport | [transport/drivers.md](transport/drivers.md) | `Vehicle_masters/vehicle_drivers` | done |
| 266 | Vehicle Report | reports | [reports/vehicle-report.md](reports/vehicle-report.md) | `veh_reports/Trip_reports/listall` | done |
| 277 | Visa Types | transport | [transport/visa-types.md](transport/visa-types.md) | `Vehicle_masters/visa_type` | done |
| 371 | BoM List | production | [production/bom-list.md](production/bom-list.md) | `production/bom/bom_controller/listall` | done |
| 372 | Status | production | [production/status.md](production/status.md) | `production/status/Status_controller/listall` | done |
| 373 | Stage | production | [production/stage.md](production/stage.md) | `production/stage/stage_controller/listall` | done |
| 374 | Additional Fileds | production | [production/additional-fileds.md](production/additional-fileds.md) | `production/label/label_controller/listall` | done |
| 375 | Process | production | [production/process.md](production/process.md) | `production/process/process_controller/listall` | done |
| 379 | Couser Master | education | [education/couser-master.md](education/couser-master.md) | `education/Course/listall` | done |
| 380 | Student | education | [education/student.md](education/student.md) | `education/Student/listall` | done |
| 381 | Discount | education | [education/discount.md](education/discount.md) | `education/Course/listDiscount` | done |
| 382 | Batches | education | [education/batches.md](education/batches.md) | `education/Student/batch` | done |
| 383 | Fees | education | [education/fees.md](education/fees.md) | `education/Student/feeStructure` | done |
| 384 | University | education | [education/university.md](education/university.md) | `education/Course/university` | done |
| 386 | Document Master | education | [education/document-master.md](education/document-master.md) | `education/Student/listDoc` | done |
| 441 | Job Card In | production | [production/job-card-in.md](production/job-card-in.md) | `Jobcard/list_all/in` | done |
| 442 | Job Card Out | production | [production/job-card-out.md](production/job-card-out.md) | `Jobcard/list_all/out` | done |
| 443 | Material In | production | [production/material-in.md](production/material-in.md) | `Material/list_all/in` | done |
| 444 | Material Out | production | [production/material-out.md](production/material-out.md) | `Material/list_all/out` | done |
| 458 | MELTING | gold | [gold/melting.md](gold/melting.md) | `gold/Melting/list_all` | done |
| 459 | Production | gold | [gold/production.md](gold/production.md) | `gold/Production/list_all` | done |
| 460 | Design | gold | [gold/design.md](gold/design.md) | `gold/Design/list_all` | done |
| 462 | Sales And Jobcard | gold | [gold/sales-and-jobcard.md](gold/sales-and-jobcard.md) | `gold/SalesJob/list_all` | done |

## Navigation folders

| ID | Screen | Folder | Spec file | Legacy URL | Status |
|----|--------|--------|-----------|------------|--------|
| 13 | Accounts Master | — | — | (folder) | folder |
| 15 | General Masters | — | — | (folder) | folder |
| 24 | Inventory Management | — | — | (folder) | folder |
| 29 | Transactions | — | — | (folder) | folder |
| 36 | Financial Reports | — | — | (folder) | folder |
| 50 | Settings | — | — | (folder) | folder |
| 115 | Inventory Reports | — | — | (folder) | folder |
| 120 | GST Reports | — | — | (folder) | folder |
| 123 | General Settings | — | — | (folder) | folder |
| 290 | SMS Master | — | — | (folder) | folder |
| 291 | User Settings | — | — | (folder) | folder |
| 292 | Sales | — | — | (folder) | folder |
| 293 | Purchase | — | — | (folder) | folder |
| 294 | Vouchers | — | — | (folder) | folder |
| 295 | Sales Reports | — | — | (folder) | folder |
| 296 | Purchase Reports | — | — | (folder) | folder |
| 297 | Item Reports | — | — | (folder) | folder |
| 317 | HR | — | — | (folder) | folder |
| 329 | CRM | — | — | (folder) | folder |
| 332 | Student Crm | — | — | (folder) | folder |
| 335 | CRM Transactions | — | — | (folder) | folder |
| 336 | CRM Reports | — | — | (folder) | folder |
| 337 | CRM Masters | — | — | (folder) | folder |
| 352 | Service CRM | — | — | (folder) | folder |
| 370 | Production | — | — | (folder) | folder |
| 393 | Export Report | — | — | (folder) | folder |
| 417 | MIS Reports | — | — | (folder) | folder |
| 461 | Gold | — | — | (folder) | folder |
| 483 | General Reports | — | — | (folder) | folder |
| 484 | GSTR1 Reports | — | — | (folder) | folder |
| 485 | GSTR2 Reports | — | — | (folder) | folder |
| 488 | DashBoards | — | — | (folder) | folder |

## Wave 4 — APIs (not menu-driven)

| Spec file | Domain |
|-----------|--------|
| [api/master-device-sync.md](api/master-device-sync.md) | Device/master sync |
| [api/sales.md](api/sales.md) | Mobile sales create |
| [api/vouchers-payment.md](api/vouchers-payment.md) | Payment/receipt vouchers |
| [api/replacement.md](api/replacement.md) | Replacement vouchers |
| [api/ecommerce.md](api/ecommerce.md) | Storefront catalog/user |
| [api/clinic-sync.md](api/clinic-sync.md) | Clinic bidirectional sync |
| [api/mobile-web.md](api/mobile-web.md) | Session-based mobile web |

## Extra auth/dashboard pages (always required)

| Spec file | Notes |
|-----------|-------|
| [auth/login.md](auth/login.md) | Default entry, not in menus table |
| [dashboard/super-admin.md](dashboard/super-admin.md) | Role landing |
| [dashboard/admin.md](dashboard/admin.md) | Role landing |
| [dashboard/users.md](dashboard/users.md) | Role landing |
| [dashboard/clinic.md](dashboard/clinic.md) | Clinic role landing |
| [dashboard/summary.md](dashboard/summary.md) | Admin summary KPI dashboard |


