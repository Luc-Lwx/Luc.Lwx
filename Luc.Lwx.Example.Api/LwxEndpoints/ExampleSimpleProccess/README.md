# Simple Process Example (Recommended Way)

```
GET /apimanager-prefix/example-simple-process/list?name_filter=xxxx
    REQ: {

    }
    RES {
        // list of processes
    }


POST /apimanager-prefix/example-simple-process/start
    REQ: {
        "abc":129738,
    }
    RES: {
        "ProcId": 123
    }

GET /apimanager-prefix/example-simple-process/status?proc_id=123
    REQ:{

    }
    RES: {
        // proc data
    }

POST /apimanager-prefix/example-simple-process/step1?proc_id=123
    REQ: {    
        "cde": 12873
    }
    RES: {
        "ok": true
    }
POST /apimanager-prefix/example-simple-process/step2?proc_id=123
    REQ: {
        "def": 219837,
        "ghi": 129873
    }
    RES: {
        "ok": true
    }
POST /apimanager-prefix/example-simple-process/finish?proc_id=123
    REQ: {
        "jkl": 2109379,
        "mno": 21873
    }
    RES: {
        "ok": true
    }
POST /apimanager-prefix/example-simple-process/cancel?proc_id=123
    REQ: {
        "are_you_sure": true        
    }
    RES: {
        "ok": true
    }
```

# WHY THIS IS RECOMMENDED?

### Prefix based routing is more efficient (and cheaper)

To support parameters on the path, API Managers and Frameworks 
have to resort to regular expressions which have a far lower 
capacity than prefix based routing.

### Poor market support for RESTful APIs

Most monitoring tools and languages are optimized for HTTP 
Standards rather than RESTful APIs and this makes the tracking
of errors and application debugging challenging.

Most proxies, application servers and languages limit the
http methods that can be used and the minimal common set is
GET, POST, PUT, DELETE which are insufficient to describe
well the operation on the resource and this increase the
cognitive complexity.

### Http Standard

* Parameters should be in the request body or in the query string
* Method describe caching, parameter format and idempotency

### Improved maintainability

Parameter values in the request-path can't be easily extracted 
from the request logs because the parameter value can have 
ambiguity and be confused with the action name/object:

Example 1:
PUT /user-management/mark/flags/exec1

Example 2:
PUT /user/flags/change?user-id=mark 
REQ: { 
    "flags": ["exec1"] 
}
```

The Example 2 is far easier to comprehend:
- it is an operation that change user flags
- the user-id is mark
- the modified flag is exec1

In the Example 1, it is unclear that
- mark is the user-id because it resembles an operation name
- exec1 is the flag because it resembles an operation name

### Improved integration

Backoffice, Control, Risk, Security and Auditing needs to parse 
the request logs to populate operational databases and this is
easier when parameters can easily separated from the action 

### Improved code review 

Will be easier for Code Reviewers to understand what the operation do. 
  
### Improved incident handling

Will be easier for Developers to understand what is going
on from the logs and treat the error reports.