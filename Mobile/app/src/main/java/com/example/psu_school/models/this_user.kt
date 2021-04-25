package com.example.psu_school.models

data class this_user(
    var Name: String = "",
    var Surname: String = "",
    var Midname: String = "",
    var Group: String = "",
    var Login: String = "Admin",
    var Password: String = "Admin",
    var Url_file: String = "",
    var QR_code: String = "",
    var Longitude: Double = 0.0,
    var Latitude: Double = 0.0
)
