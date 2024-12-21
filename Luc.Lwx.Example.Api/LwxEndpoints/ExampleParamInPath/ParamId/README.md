# Simple Proccess (Old Way)

Many developers like to put parameters in the request_path, but we do not like that because it compromises the performance and maintainability of the application.

Having said that, this example includes the handling of a small proccess:

POST /apimanager-prefix/example-param-in-path/{id}/start
PUT /apimanager-prefix/example-param-in-path/{id}/step1
PUT /apimanager-prefix/example-param-in-path/{id}/step2
DELETE /apimanager-prefix/example-param-in-path/{id}/cancel
PUT /apimanager-prefix/example-param-in-path/{id}/finish


