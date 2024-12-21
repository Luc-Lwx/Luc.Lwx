# Simple Process Example (Recommended Way)

## Endpoint List

### Endpoint: List Example Processes
**Class:** `<assembly>.LwxEndpoints.ExampleSimpleProccess.EndpointList`

**Request:** `GET /apimanager-prefix/example-simple-process/list?name_filter=xxxx`

**Response:**
```json
{
    "ok": true,
    "proc_list": [
        {
            "id": 2109381218923719,
            "started_at": "2024-12-31T15:45:22.0000-03:00"                
        },
        {
            "id": 210909480248094,
            "started_at": "2024-11-221T12:44:32.0000-03:00"                
        }            
    ]
}
```

## Endpoint Start

### Endpoint: Start Example Process
**Class:** `<assembly>.LwxEndpoints.ExampleSimpleProccess.EndpointStart`

**Request:** `POST /apimanager-prefix/example-simple-process/start`

**Request Body:**
```json
{
    "name": "JOHN DOE"
}
```

**Response:**
```json
{
    "ok": true,
    "proc-id": 210909480248094
}
```

## Endpoint Status

### Endpoint: Get Example Process Status
**Class:** `<assembly>.LwxEndpoints.ExampleSimpleProccess.EndpointStatus`

**Request:** `GET /apimanager-prefix/example-simple-process/status?proc_id=210909480248094`

**Response:**
```json
{
    "ok": true,
    "status": "open"
}
```

## Endpoint Step 1

### Endpoint: Execute Step 1 of Example Process
**Class:** `<assembly>.LwxEndpoints.ExampleSimpleProccess.EndpointStep1`

**Request:** `POST /apimanager-prefix/example-simple-process/step1?proc_id=210909480248094`

**Request Body:**
```json
{
    "param1": 12873,
    "param2": "120983109aaa",
    "param3": "981273987"
}
```

**Response:**
```json
{
    "ok": true
}
```

## Endpoint Step 2

### Endpoint: Execute Step 2 of Example Process
**Class:** `<assembly>.LwxEndpoints.ExampleSimpleProccess.EndpointStep2`

**Request:** `POST /apimanager-prefix/example-simple-process/step2?proc_id=210909480248094`

**Request Body:**
```json
{
    "param4": 9237912789,
    "param5": "210948-0824019",
    "param6": "1-20401942"
}
```

**Response:**
```json
{
    "ok": true
}
```

## Endpoint Finish

### Endpoint: Finish Example Process
**Class:** `<assembly>.LwxEndpoints.ExampleSimpleProccess.EndpointFinish`

**Request:** ```POST /apimanager-prefix/example-simple-process/finish?proc_id=210909480248094```

**Request Body:**
```json
{
    "param7": 2109379,
    "param8": 21873,
    "accepted": true
}
```

**Response:**
```json
{
    "ok": true
}
```

## Endpoint Cancel

### Endpoint: Cancel Example Process
**Class:** `<assembly>.LwxEndpoints.ExampleSimpleProccess.EndpointCancel`

**Request:** `POST /apimanager-prefix/example-simple-process/cancel?proc_id=210909480248094`

**Request Body:**
```json
{
    "are_you_sure": true        
}
```

**Response:**
```json
{
    "ok": true
}
```

# WHY THIS IS RECOMMENDED?

### Prefix based routing is more efficient (and cheaper)

To support parameters in the path, API Managers and Frameworks have to resort to regular expressions, which have a far lower capacity than prefix-based routing.

### Poor market support for RESTful APIs

Most monitoring tools and languages are optimized for HTTP standards rather than RESTful APIs, making error tracking and debugging challenging. Most proxies, application servers, and languages limit the HTTP methods that can be used, and the minimal common set is GET, POST, PUT, DELETE, which are insufficient to describe operations well, increasing cognitive complexity.

### HTTP Standard

- Parameters should be in the request body or in the query string.
- Methods describe caching, parameter format, and idempotency.

### Improved maintainability

Parameter values in the request path can't be easily extracted from the request logs because the parameter value can have ambiguity and be confused with the action name/object.

**Example 1:**
PUT /user-management/mark/flags/exec1

**Example 2:**
PUT /user/flags/change?user-id=mark 
REQ: { 
    "flags": ["exec1"] 
}

The Example 2 is far easier to comprehend:
- it is an operation that change user flags
- the user-id is mark
- the modified flag is exec1

In the Example 1, it is unclear that
- mark is the user-id because it resembles an operation name
- exec1 is the flag because it resembles an operation name

### Improved integration

Backoffice, Control, Risk, Security and Auditing needs to parse the request logs to populate operational databases and this is easier when parameters can easily separated from the action 

### Improved code Review 

Will be easier for Code Reviewers to understand what the operation do. 
  
### Improved incident Handling

Will be easier for Developers to understand what is going
on from the logs and treat the error reports.