const { TableClient, AzureSASCredential } = require("@azure/data-tables");
const connectMiddleware = require("./connectMiddleware");

describe("connectMiddleware", () => {
  let req, res, next;

  beforeEach(() => {
    req = {};
    res = {
      status: jest.fn().mockReturnThis(),
      json: jest.fn(),
    };
    next = jest.fn();
  });

  afterEach(() => {
    jest.clearAllMocks();
  });

  it("should set the tableClient on the request object and call next", () => {
    process.env.AZURE_TABLE_ACCOUNT = "testAccount";
    process.env.AZURE_TABLE_SAS = "testSAS";
    process.env.AZURE_TABLE_TABLENAME = "testTable";

    connectMiddleware(req, res, next);

    expect(req.tableClient).toBeInstanceOf(TableClient);
    expect(req.tableClient.url).toBe(
      "https://testAccount.table.core.windows.net"
    );
    expect(next).toHaveBeenCalled();
  });

  it("should return 500 if environment variables are missing", () => {
    delete process.env.AZURE_TABLE_ACCOUNT;
    delete process.env.AZURE_TABLE_SAS;
    delete process.env.AZURE_TABLE_TABLENAME;

    connectMiddleware(req, res, next);

    expect(res.status).toHaveBeenCalledWith(500);
    expect(res.json).toHaveBeenCalledWith({
      error: "Missing required environment variables",
    });
    expect(next).not.toHaveBeenCalled();
  });

  it("should return 500 if only some environment variables are missing", () => {
    process.env.AZURE_TABLE_ACCOUNT = "testAccount";
    delete process.env.AZURE_TABLE_SAS;
    process.env.AZURE_TABLE_TABLENAME = "testTable";

    connectMiddleware(req, res, next);

    expect(res.status).toHaveBeenCalledWith(500);
    expect(res.json).toHaveBeenCalledWith({
      error: "Missing required environment variables",
    });
    expect(next).not.toHaveBeenCalled();
  });
});