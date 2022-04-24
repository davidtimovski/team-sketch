CREATE TABLE public.events
(
    id serial NOT NULL,
	room_id integer NOT NULL,
	connection_id integer NOT NULL,
	type smallint NOT NULL,
    occurred timestamp with time zone NOT NULL,
	CONSTRAINT "PK_events" PRIMARY KEY (id),
    CONSTRAINT "FK_events_rooms_room_id" FOREIGN KEY (room_id)
    REFERENCES public.rooms (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE CASCADE
	DEFERRABLE INITIALLY DEFERRED,
	CONSTRAINT "FK_events_connections_connection_id" FOREIGN KEY (connection_id)
    REFERENCES public.connections (id) MATCH SIMPLE
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
	