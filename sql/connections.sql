CREATE TABLE public.connections
(
    id serial NOT NULL,
	room_id integer NOT NULL,
	signalr_connection_id character varying(30) NOT NULL COLLATE pg_catalog."default",
	ip_address character varying(15) COLLATE pg_catalog."default",
    "user" character varying(30) NOT NULL COLLATE pg_catalog."default",
	is_connected boolean NOT NULL DEFAULT TRUE,
    created timestamp with time zone NOT NULL,
	modified timestamp with time zone NOT NULL,
	CONSTRAINT "PK_connections" PRIMARY KEY (id),
    CONSTRAINT "FK_connections_rooms_room_id" FOREIGN KEY (room_id)
    REFERENCES public.rooms (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE
	DEFERRABLE INITIALLY DEFERRED
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.rooms
    OWNER to postgres;
	
CREATE INDEX "IX_connections_signalr_connection_id"
    ON public.connections USING btree
    (signalr_connection_id)
    TABLESPACE pg_default;
