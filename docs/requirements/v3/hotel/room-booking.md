# Hotel Room Booking

## 1. Identity
- Module / area: Hotel
- Page type: List | Create | Edit
- Primary users / roles: Front desk
- Entry points: `hotel/Roombooking/list_all`
- Priority for rebuild: P2

## 2. Purpose
Reserve and check in guests across one or more rooms with rates, tax, extra beds, advances, and stay duration.

## 3. Preconditions
- Floors, room types, categories, rates, rooms, customer/guest

## 4. Layout and UI
- Booking list; create/check-in forms
- Multi-room grid on booking
- ID proof uploads

## 5. Fields (detailed)
| Field label | Internal name | Control | Required | Default | Validation | Options / lookup source | Visible when | Notes |
|-------------|---------------|---------|----------|---------|------------|-------------------------|--------------|-------|
| Guest / Customer | customer_id | lookup | yes | — | — | suppliers/customers | always | |
| Address / Phone / Contact | address, phone, contact | text | yes | — | — | — | always | |
| ID proof images | image_a, id_proof_a | upload | no | — | file | — | always | |
| Stay dates | start_date, end_date | date | yes | — | end≥start | — | always | |
| Days | no_of_days | calc | — | — | — | — | always | |
| Advance / Total / Pending | advance, total, pending_amount | number | yes | 0 | >=0 | — | always | |
| Room lines | room_floor, room_roomtype, room_id, tax_id, extra_beds, rate, bed rates, row_total | grid | yes | — | room available | hotel masters | always | |

## 6. Actions and flows
- Create reservation → check-in → settle pending → checkout (define explicitly in new system)

## 7. Business rules
- Room availability by date range
- Tax on room rate; extra bed charges
- Pending = total − advance

## 8. Data requirements
- HotelBooking, BookingRoomLine, Room, RoomType, RoomCategory, RoomRate, Floor, Customer

## 9. Integrations
- File uploads; optional folio PDF

## 10. Permissions
- Hotel menus

## 11. Reports / exports
- Occupancy (recommended new); booking list export

## 12. Edge cases
- Overlapping bookings same room; early checkout

## 13. Acceptance criteria
- Book available room for date range
- Conflict rejected
- Pending amount computes correctly

## 14. Legacy reference
- Controllers: `hotel/Roombooking.php`, `Room.php`, `Roomtype.php`, `Roomcategory.php`, `Roomrate.php`, `Floor.php`
- Tables: `hotel_booking`, `hotel_room*`
